// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using CommunityToolkit.Mvvm.SourceGenerators.ComponentModel.Models;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using CommunityToolkit.Mvvm.SourceGenerators.Helpers;
using CommunityToolkit.Mvvm.SourceGenerators.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <inheritdoc/>
partial class ObservablePropertyGenerator
{
    /// <summary>
    /// A container for all the logic for <see cref="ObservablePropertyGenerator"/>.
    /// </summary>
    internal static class Execute
    {
        /// <summary>
        /// Processes a given field.
        /// </summary>
        /// <param name="fieldSyntax">The <see cref="FieldDeclarationSyntax"/> instance to process.</param>
        /// <param name="fieldSymbol">The input <see cref="IFieldSymbol"/> instance to process.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/> instance for the current run.</param>
        /// <param name="token">The cancellation token for the current operation.</param>
        /// <param name="propertyInfo">The resulting <see cref="PropertyInfo"/> value, if successfully retrieved.</param>
        /// <param name="diagnostics">The resulting diagnostics from the processing operation.</param>
        /// <returns>The resulting <see cref="PropertyInfo"/> instance for <paramref name="fieldSymbol"/>, if successful.</returns>
        public static bool TryGetInfo(
            FieldDeclarationSyntax fieldSyntax,
            IFieldSymbol fieldSymbol,
            SemanticModel semanticModel,
            CancellationToken token,
            [NotNullWhen(true)] out PropertyInfo? propertyInfo,
            out ImmutableArray<DiagnosticInfo> diagnostics)
        {
            using ImmutableArrayBuilder<DiagnosticInfo> builder = ImmutableArrayBuilder<DiagnosticInfo>.Rent();

            // Validate the target type
            if (!IsTargetTypeValid(fieldSymbol, out bool shouldInvokeOnPropertyChanging))
            {
                builder.Add(
                    InvalidContainingTypeForObservablePropertyFieldError,
                    fieldSymbol,
                    fieldSymbol.ContainingType,
                    fieldSymbol.Name);

                propertyInfo = null;
                diagnostics = builder.ToImmutable();

                return false;
            }

            // Get the property type and name
            string typeNameWithNullabilityAnnotations = fieldSymbol.Type.GetFullyQualifiedNameWithNullabilityAnnotations();
            string fieldName = fieldSymbol.Name;
            string propertyName = GetGeneratedPropertyName(fieldSymbol);

            // Check for name collisions
            if (fieldName == propertyName)
            {
                builder.Add(
                    ObservablePropertyNameCollisionError,
                    fieldSymbol,
                    fieldSymbol.ContainingType,
                    fieldSymbol.Name);

                propertyInfo = null;
                diagnostics = builder.ToImmutable();

                // If the generated property would collide, skip generating it entirely. This makes sure that
                // users only get the helpful diagnostic about the collision, and not the normal compiler error
                // about a definition for "Property" already existing on the target type, which might be confusing.
                return false;
            }

            // Check for special cases that are explicitly not allowed
            if (IsGeneratedPropertyInvalid(propertyName, fieldSymbol.Type))
            {
                builder.Add(
                    InvalidObservablePropertyError,
                    fieldSymbol,
                    fieldSymbol.ContainingType,
                    fieldSymbol.Name);

                propertyInfo = null;
                diagnostics = builder.ToImmutable();

                return false;
            }

            using ImmutableArrayBuilder<string> propertyChangedNames = ImmutableArrayBuilder<string>.Rent();
            using ImmutableArrayBuilder<string> propertyChangingNames = ImmutableArrayBuilder<string>.Rent();
            using ImmutableArrayBuilder<string> notifiedCommandNames = ImmutableArrayBuilder<string>.Rent();
            using ImmutableArrayBuilder<AttributeInfo> forwardedAttributes = ImmutableArrayBuilder<AttributeInfo>.Rent();

            bool notifyRecipients = false;
            bool notifyDataErrorInfo = false;
            bool hasOrInheritsClassLevelNotifyPropertyChangedRecipients = false;
            bool hasOrInheritsClassLevelNotifyDataErrorInfo = false;
            bool hasAnyValidationAttributes = false;
            bool isOldPropertyValueDirectlyReferenced = IsOldPropertyValueDirectlyReferenced(fieldSymbol, propertyName);
            bool isReferenceType = fieldSymbol.Type.IsReferenceType;

            // Track the property changing event for the property, if the type supports it
            if (shouldInvokeOnPropertyChanging)
            {
                propertyChangingNames.Add(propertyName);
            }

            // The current property is always notified
            propertyChangedNames.Add(propertyName);

            // Get the class-level [NotifyPropertyChangedRecipients] setting, if any
            if (TryGetIsNotifyingRecipients(fieldSymbol, out bool isBroadcastTargetValid))
            {
                notifyRecipients = isBroadcastTargetValid;
                hasOrInheritsClassLevelNotifyPropertyChangedRecipients = true;
            }

            // Get the class-level [NotifyDataErrorInfo] setting, if any
            if (TryGetNotifyDataErrorInfo(fieldSymbol, out bool isValidationTargetValid))
            {
                notifyDataErrorInfo = isValidationTargetValid;
                hasOrInheritsClassLevelNotifyDataErrorInfo = true;
            }

            // Gather attributes info
            foreach (AttributeData attributeData in fieldSymbol.GetAttributes())
            {
                // Gather dependent property and command names
                if (TryGatherDependentPropertyChangedNames(fieldSymbol, attributeData, in propertyChangedNames, in builder) ||
                    TryGatherDependentCommandNames(fieldSymbol, attributeData, in notifiedCommandNames, in builder))
                {
                    continue;
                }

                // Check whether the property should also notify recipients
                if (TryGetIsNotifyingRecipients(fieldSymbol, attributeData, in builder, hasOrInheritsClassLevelNotifyPropertyChangedRecipients, out isBroadcastTargetValid))
                {
                    notifyRecipients = isBroadcastTargetValid;

                    continue;
                }

                // Check whether the property should also be validated
                if (TryGetNotifyDataErrorInfo(fieldSymbol, attributeData, in builder, hasOrInheritsClassLevelNotifyDataErrorInfo, out isValidationTargetValid))
                {
                    notifyDataErrorInfo = isValidationTargetValid;

                    continue;
                }

                // Track the current attribute for forwarding if it is a validation attribute
                if (attributeData.AttributeClass?.InheritsFromFullyQualifiedMetadataName("System.ComponentModel.DataAnnotations.ValidationAttribute") == true)
                {
                    hasAnyValidationAttributes = true;

                    forwardedAttributes.Add(AttributeInfo.From(attributeData));
                }

                // Also track the current attribute for forwarding if it is of any of the following types:
                //   - Display attributes (System.ComponentModel.DataAnnotations.DisplayAttribute)
                //   - UI hint attributes(System.ComponentModel.DataAnnotations.UIHintAttribute)
                //   - Scaffold column attributes (System.ComponentModel.DataAnnotations.ScaffoldColumnAttribute)
                //   - Editable attributes (System.ComponentModel.DataAnnotations.EditableAttribute)
                //   - Key attributes (System.ComponentModel.DataAnnotations.KeyAttribute)
                if (attributeData.AttributeClass?.HasOrInheritsFromFullyQualifiedMetadataName("System.ComponentModel.DataAnnotations.UIHintAttribute") == true ||
                    attributeData.AttributeClass?.HasOrInheritsFromFullyQualifiedMetadataName("System.ComponentModel.DataAnnotations.ScaffoldColumnAttribute") == true ||
                    attributeData.AttributeClass?.HasFullyQualifiedMetadataName("System.ComponentModel.DataAnnotations.DisplayAttribute") == true ||
                    attributeData.AttributeClass?.HasFullyQualifiedMetadataName("System.ComponentModel.DataAnnotations.EditableAttribute") == true ||
                    attributeData.AttributeClass?.HasFullyQualifiedMetadataName("System.ComponentModel.DataAnnotations.KeyAttribute") == true)
                {
                    forwardedAttributes.Add(AttributeInfo.From(attributeData));
                }
            }

            // Gather explicit forwarded attributes info
            foreach (AttributeListSyntax attributeList in fieldSyntax.AttributeLists)
            {
                // Only look for attribute lists explicitly targeting the (generated) property. Roslyn will normally emit a
                // CS0657 warning (invalid target), but that is automatically suppressed by a dedicated diagnostic suppressor
                // that recognizes uses of this target specifically to support [ObservableProperty].
                if (attributeList.Target?.Identifier.Kind() is not SyntaxKind.PropertyKeyword)
                {
                    continue;
                }

                foreach (AttributeSyntax attribute in attributeList.Attributes)
                {
                    // Roslyn ignores attributes in an attribute list with an invalid target, so we can't get the AttributeData as usual.
                    // To reconstruct all necessary attribute info to generate the serialized model, we use the following steps:
                    //   - We try to get the attribute symbol from the semantic model, for the current attribute syntax. In case this is not
                    //     available (in theory it shouldn't, but it can be), we try to get it from the candidate symbols list for the node.
                    //     If there are no candidates or more than one, we just issue a diagnostic and stop processing the current attribute.
                    //     The returned symbols might be method symbols (constructor attribute) so in that case we can get the declaring type.
                    //   - We then go over each attribute argument expression and get the operation for it. This will still be available even
                    //     though the rest of the attribute is not validated nor bound at all. From the operation we can still retrieve all
                    //     constant values to build the AttributeInfo model. After all, attributes only support constant values, typeof(T)
                    //     expressions, or arrays of either these two types, or of other arrays with the same rules, recursively.
                    //   - From the syntax, we can also determine the identifier names for named attribute arguments, if any.
                    // There is no need to validate anything here: the attribute will be forwarded as is, and then Roslyn will validate on the
                    // generated property. Users will get the same validation they'd have had directly over the field. The only drawback is the
                    // lack of IntelliSense when constructing attributes over the field, but this is the best we can do from this end anyway.
                    SymbolInfo attributeSymbolInfo = semanticModel.GetSymbolInfo(attribute, token);

                    // Check if the attribute type can be resolved, and emit a diagnostic if that is not the case. This happens if eg. the
                    // attribute type is spelled incorrectly, or if a user is missing the using directive for the attribute type being used.
                    if ((attributeSymbolInfo.Symbol ?? attributeSymbolInfo.CandidateSymbols.SingleOrDefault()) is not ISymbol attributeSymbol ||
                        (attributeSymbol as INamedTypeSymbol ?? attributeSymbol.ContainingType) is not INamedTypeSymbol attributeTypeSymbol)
                    {
                        builder.Add(
                            InvalidPropertyTargetedAttributeOnObservablePropertyField,
                            attribute,
                            fieldSymbol,
                            attribute.Name);

                        continue;
                    }

                    forwardedAttributes.Add(AttributeInfo.From(attributeTypeSymbol, semanticModel, attribute.ArgumentList?.Arguments ?? Enumerable.Empty<AttributeArgumentSyntax>(), token));
                }
            }

            // Log the diagnostic for missing ObservableValidator, if needed
            if (hasAnyValidationAttributes &&
                !fieldSymbol.ContainingType.InheritsFromFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservableValidator"))
            {
                builder.Add(
                    MissingObservableValidatorInheritanceForValidationAttributeError,
                    fieldSymbol,
                    fieldSymbol.ContainingType,
                    fieldSymbol.Name,
                    forwardedAttributes.Count);
            }

            // Log the diagnostic for missing validation attributes, if any
            if (notifyDataErrorInfo && !hasAnyValidationAttributes)
            {
                builder.Add(
                    MissingValidationAttributesForNotifyDataErrorInfoError,
                    fieldSymbol,
                    fieldSymbol.ContainingType,
                    fieldSymbol.Name);
            }

            propertyInfo = new PropertyInfo(
                typeNameWithNullabilityAnnotations,
                fieldName,
                propertyName,
                propertyChangingNames.ToImmutable(),
                propertyChangedNames.ToImmutable(),
                notifiedCommandNames.ToImmutable(),
                notifyRecipients,
                notifyDataErrorInfo,
                isOldPropertyValueDirectlyReferenced,
                isReferenceType,
                forwardedAttributes.ToImmutable());

            diagnostics = builder.ToImmutable();

            return true;
        }

        /// <summary>
        /// Validates the containing type for a given field being annotated.
        /// </summary>
        /// <param name="fieldSymbol">The input <see cref="IFieldSymbol"/> instance to process.</param>
        /// <param name="shouldInvokeOnPropertyChanging">Whether or not property changing events should also be raised.</param>
        /// <returns>Whether or not the containing type for <paramref name="fieldSymbol"/> is valid.</returns>
        private static bool IsTargetTypeValid(IFieldSymbol fieldSymbol, out bool shouldInvokeOnPropertyChanging)
        {
            // The [ObservableProperty] attribute can only be used in types that are known to expose the necessary OnPropertyChanged and OnPropertyChanging methods.
            // That means that the containing type for the field needs to match one of the following conditions:
            //   - It inherits from ObservableObject (in which case it also implements INotifyPropertyChanging).
            //   - It has the [ObservableObject] attribute (on itself or any of its base types).
            //   - It has the [INotifyPropertyChanged] attribute (on itself or any of its base types).
            bool isObservableObject = fieldSymbol.ContainingType.InheritsFromFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservableObject");
            bool hasObservableObjectAttribute = fieldSymbol.ContainingType.HasOrInheritsAttributeWithFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservableObjectAttribute");
            bool hasINotifyPropertyChangedAttribute = fieldSymbol.ContainingType.HasOrInheritsAttributeWithFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.INotifyPropertyChangedAttribute");

            shouldInvokeOnPropertyChanging = isObservableObject || hasObservableObjectAttribute;

            return isObservableObject || hasObservableObjectAttribute || hasINotifyPropertyChangedAttribute;
        }

        /// <summary>
        /// Checks whether the generated property would be a special case that is marked as invalid.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="propertyType">The property type.</param>
        /// <returns>Whether the generated property is invalid.</returns>
        private static bool IsGeneratedPropertyInvalid(string propertyName, ITypeSymbol propertyType)
        {
            // If the generated property name is called "Property" and the type is either object or it is PropertyChangedEventArgs or
            // PropertyChangingEventArgs (or a type derived from either of those two types), consider it invalid. This is needed because
            // if such a property was generated, the partial On<PROPERTY_NAME>Changing and OnPropertyChanging(PropertyChangingEventArgs)
            // methods, as well as the partial On<PROPERTY_NAME>Changed and OnPropertyChanged(PropertyChangedEventArgs) methods.
            if (propertyName == "Property")
            {
                return
                    propertyType.SpecialType == SpecialType.System_Object ||
                    propertyType.HasOrInheritsFromFullyQualifiedMetadataName("System.ComponentModel.PropertyChangedEventArgs") ||
                    propertyType.HasOrInheritsFromFullyQualifiedMetadataName("System.ComponentModel.PropertyChangingEventArgs");
            }

            return false;
        }

        /// <summary>
        /// Tries to gather dependent properties from the given attribute.
        /// </summary>
        /// <param name="fieldSymbol">The input <see cref="IFieldSymbol"/> instance to process.</param>
        /// <param name="attributeData">The <see cref="AttributeData"/> instance for <paramref name="fieldSymbol"/>.</param>
        /// <param name="propertyChangedNames">The target collection of dependent property names to populate.</param>
        /// <param name="diagnostics">The current collection of gathered diagnostics.</param>
        /// <returns>Whether or not <paramref name="attributeData"/> was an attribute containing any dependent properties.</returns>
        private static bool TryGatherDependentPropertyChangedNames(
            IFieldSymbol fieldSymbol,
            AttributeData attributeData,
            in ImmutableArrayBuilder<string> propertyChangedNames,
            in ImmutableArrayBuilder<DiagnosticInfo> diagnostics)
        {
            // Validates a property name using existing properties
            bool IsPropertyNameValid(string propertyName)
            {
                return fieldSymbol.ContainingType.GetAllMembers(propertyName).OfType<IPropertySymbol>().Any();
            }

            // Validate a property name including generated properties too
            bool IsPropertyNameValidWithGeneratedMembers(string propertyName)
            {
                foreach (ISymbol member in fieldSymbol.ContainingType.GetAllMembers())
                {
                    if (member is IFieldSymbol otherFieldSymbol &&
                        !SymbolEqualityComparer.Default.Equals(fieldSymbol, otherFieldSymbol) &&
                        otherFieldSymbol.HasAttributeWithFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute") &&
                        propertyName == GetGeneratedPropertyName(otherFieldSymbol))
                    {
                        return true;
                    }
                }

                return false;
            }

            if (attributeData.AttributeClass?.HasFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.NotifyPropertyChangedForAttribute") == true)
            {
                foreach (string? dependentPropertyName in attributeData.GetConstructorArguments<string>())
                {
                    // Each target must be a (not null and not empty) string matching the name of a property from the containing type
                    // of the annotated field being processed, or alternatively it must match the name of a property being generated.
                    if (dependentPropertyName is not (null or "") &&
                        (IsPropertyNameValid(dependentPropertyName) ||
                         IsPropertyNameValidWithGeneratedMembers(dependentPropertyName)))
                    {
                        propertyChangedNames.Add(dependentPropertyName);
                    }
                    else
                    {
                        diagnostics.Add(
                            NotifyPropertyChangedForInvalidTargetError,
                            fieldSymbol,
                            dependentPropertyName ?? "",
                            fieldSymbol.ContainingType);
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to gather dependent commands from the given attribute.
        /// </summary>
        /// <param name="fieldSymbol">The input <see cref="IFieldSymbol"/> instance to process.</param>
        /// <param name="attributeData">The <see cref="AttributeData"/> instance for <paramref name="fieldSymbol"/>.</param>
        /// <param name="notifiedCommandNames">The target collection of dependent command names to populate.</param>
        /// <param name="diagnostics">The current collection of gathered diagnostics.</param>
        /// <returns>Whether or not <paramref name="attributeData"/> was an attribute containing any dependent commands.</returns>
        private static bool TryGatherDependentCommandNames(
            IFieldSymbol fieldSymbol,
            AttributeData attributeData,
            in ImmutableArrayBuilder<string> notifiedCommandNames,
            in ImmutableArrayBuilder<DiagnosticInfo> diagnostics)
        {
            // Validates a command name using existing properties
            bool IsCommandNameValid(string commandName, out bool shouldLookForGeneratedMembersToo)
            {
                // Each target must be a string matching the name of a property from the containing type of the annotated field, and the
                // property must be of type IRelayCommand, or any type that implements that interface (to avoid generating invalid code).
                if (fieldSymbol.ContainingType.GetAllMembers(commandName).OfType<IPropertySymbol>().FirstOrDefault() is IPropertySymbol propertySymbol)
                {
                    // If there is a property member with the specified name, check that it's valid. If it isn't, the
                    // target is definitely not valid, and the additional checks below can just be skipped. The property
                    // is valid if it's of type IRelayCommand, or it has IRelayCommand in the set of all interfaces.
                    if (propertySymbol.Type is INamedTypeSymbol typeSymbol &&
                        (typeSymbol.HasFullyQualifiedMetadataName("CommunityToolkit.Mvvm.Input.IRelayCommand") ||
                         typeSymbol.HasInterfaceWithFullyQualifiedMetadataName("CommunityToolkit.Mvvm.Input.IRelayCommand")))
                    {
                        shouldLookForGeneratedMembersToo = true;

                        return true;
                    }

                    // If a property with this name exists but is not valid, the search should stop immediately, as
                    // the target is already known not to be valid, so there is no reason to look for other members.
                    shouldLookForGeneratedMembersToo = false;

                    return false;
                }

                shouldLookForGeneratedMembersToo = true;

                return false;
            }

            // Validate a command name including generated command too
            bool IsCommandNameValidWithGeneratedMembers(string commandName)
            {
                foreach (ISymbol member in fieldSymbol.ContainingType.GetAllMembers())
                {
                    if (member is IMethodSymbol methodSymbol &&
                        methodSymbol.HasAttributeWithFullyQualifiedMetadataName("CommunityToolkit.Mvvm.Input.RelayCommandAttribute") &&
                        commandName == RelayCommandGenerator.Execute.GetGeneratedFieldAndPropertyNames(methodSymbol).PropertyName)
                    {
                        return true;
                    }
                }

                return false;
            }

            if (attributeData.AttributeClass?.HasFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.NotifyCanExecuteChangedForAttribute") == true)
            {
                foreach (string? commandName in attributeData.GetConstructorArguments<string>())
                {
                    // Each command must be a (not null and not empty) string matching the name of an existing command from the containing
                    // type (just like for properties), or it must match a generated command. The only caveat is the case where a property
                    // with the requested name does exist, but it is not of the right type. In that case the search should stop immediately.
                    if (commandName is not (null or "") &&
                        (IsCommandNameValid(commandName, out bool shouldLookForGeneratedMembersToo) ||
                         shouldLookForGeneratedMembersToo && IsCommandNameValidWithGeneratedMembers(commandName)))
                    {
                        notifiedCommandNames.Add(commandName);
                    }
                    else
                    {
                        diagnostics.Add(
                           NotifyCanExecuteChangedForInvalidTargetError,
                           fieldSymbol,
                           commandName ?? "",
                           fieldSymbol.ContainingType);
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks whether a given generated property should also notify recipients.
        /// </summary>
        /// <param name="fieldSymbol">The input <see cref="IFieldSymbol"/> instance to process.</param>
        /// <param name="isBroadcastTargetValid">Whether or not the the property is in a valid target that can notify recipients.</param>
        /// <returns>Whether or not the generated property for <paramref name="fieldSymbol"/> is in a type annotated with <c>[NotifyPropertyChangedRecipients]</c>.</returns>
        private static bool TryGetIsNotifyingRecipients(IFieldSymbol fieldSymbol, out bool isBroadcastTargetValid)
        {
            if (fieldSymbol.ContainingType?.HasOrInheritsAttributeWithFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.NotifyPropertyChangedRecipientsAttribute") == true)
            {
                // If the containing type is valid, track it
                if (fieldSymbol.ContainingType.InheritsFromFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservableRecipient") ||
                    fieldSymbol.ContainingType.HasOrInheritsAttributeWithFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservableRecipientAttribute"))
                {
                    isBroadcastTargetValid = true;

                    return true;
                }

                // Otherwise, ignore the attribute but don't emit a diagnostic.
                // The diagnostic for class-level attributes is handled separately.
                isBroadcastTargetValid = false;

                return true;
            }

            isBroadcastTargetValid = false;

            return false;
        }

        /// <summary>
        /// Checks whether a given generated property should also notify recipients.
        /// </summary>
        /// <param name="fieldSymbol">The input <see cref="IFieldSymbol"/> instance to process.</param>
        /// <param name="attributeData">The <see cref="AttributeData"/> instance for <paramref name="fieldSymbol"/>.</param>
        /// <param name="diagnostics">The current collection of gathered diagnostics.</param>
        /// <param name="hasOrInheritsClassLevelNotifyPropertyChangedRecipients">Indicates wether the containing type of <paramref name="fieldSymbol"/> has or inherits <c>[NotifyPropertyChangedRecipients]</c>.</param>
        /// <param name="isBroadcastTargetValid">Whether or not the the property is in a valid target that can notify recipients.</param>
        /// <returns>Whether or not the generated property for <paramref name="fieldSymbol"/> used <c>[NotifyPropertyChangedRecipients]</c>.</returns>
        private static bool TryGetIsNotifyingRecipients(
            IFieldSymbol fieldSymbol,
            AttributeData attributeData,
            in ImmutableArrayBuilder<DiagnosticInfo> diagnostics,
            bool hasOrInheritsClassLevelNotifyPropertyChangedRecipients,
            out bool isBroadcastTargetValid)
        {
            if (attributeData.AttributeClass?.HasFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.NotifyPropertyChangedRecipientsAttribute") == true)
            {
                // Emit a diagnostic if the attribute is unnecessary
                if (hasOrInheritsClassLevelNotifyPropertyChangedRecipients)
                {
                    diagnostics.Add(
                        UnnecessaryNotifyPropertyChangedRecipientsAttributeOnFieldWarning,
                        fieldSymbol,
                        fieldSymbol.ContainingType,
                        fieldSymbol.Name);
                }

                // If the containing type is valid, track it
                if (fieldSymbol.ContainingType.InheritsFromFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservableRecipient") ||
                    fieldSymbol.ContainingType.HasOrInheritsAttributeWithFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservableRecipientAttribute"))
                {
                    isBroadcastTargetValid = true;

                    return true;
                }

                // Otherwise just emit the diagnostic and then ignore the attribute
                diagnostics.Add(
                    InvalidContainingTypeForNotifyPropertyChangedRecipientsFieldError,
                    fieldSymbol,
                    fieldSymbol.ContainingType,
                    fieldSymbol.Name);

                isBroadcastTargetValid = false;

                return true;
            }

            isBroadcastTargetValid = false;

            return false;
        }

        /// <summary>
        /// Checks whether a given generated property should also validate its value.
        /// </summary>
        /// <param name="fieldSymbol">The input <see cref="IFieldSymbol"/> instance to process.</param>
        /// <param name="isValidationTargetValid">Whether or not the the property is in a valid target that can validate values.</param>
        /// <returns>Whether or not the generated property for <paramref name="fieldSymbol"/> used <c>[NotifyDataErrorInfo]</c>.</returns>
        private static bool TryGetNotifyDataErrorInfo(IFieldSymbol fieldSymbol, out bool isValidationTargetValid)
        {
            if (fieldSymbol.ContainingType?.HasOrInheritsAttributeWithFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.NotifyDataErrorInfoAttribute") == true)
            {
                // If the containing type is valid, track it
                if (fieldSymbol.ContainingType.InheritsFromFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservableValidator"))
                {
                    isValidationTargetValid = true;

                    return true;
                }

                // Otherwise, ignore the attribute but don't emit a diagnostic (same as above)
                isValidationTargetValid = false;

                return true;
            }

            isValidationTargetValid = false;

            return false;
        }

        /// <summary>
        /// Checks whether a given generated property should also validate its value.
        /// </summary>
        /// <param name="fieldSymbol">The input <see cref="IFieldSymbol"/> instance to process.</param>
        /// <param name="attributeData">The <see cref="AttributeData"/> instance for <paramref name="fieldSymbol"/>.</param>
        /// <param name="diagnostics">The current collection of gathered diagnostics.</param>
        /// <param name="hasOrInheritsClassLevelNotifyDataErrorInfo">Indicates wether the containing type of <paramref name="fieldSymbol"/> has or inherits <c>[NotifyDataErrorInfo]</c>.</param>
        /// <param name="isValidationTargetValid">Whether or not the the property is in a valid target that can validate values.</param>
        /// <returns>Whether or not the generated property for <paramref name="fieldSymbol"/> used <c>[NotifyDataErrorInfo]</c>.</returns>
        private static bool TryGetNotifyDataErrorInfo(
            IFieldSymbol fieldSymbol,
            AttributeData attributeData,
            in ImmutableArrayBuilder<DiagnosticInfo> diagnostics,
            bool hasOrInheritsClassLevelNotifyDataErrorInfo,
            out bool isValidationTargetValid)
        {
            if (attributeData.AttributeClass?.HasFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.NotifyDataErrorInfoAttribute") == true)
            {
                // Emit a diagnostic if the attribute is unnecessary
                if (hasOrInheritsClassLevelNotifyDataErrorInfo)
                {
                    diagnostics.Add(
                        UnnecessaryNotifyDataErrorInfoAttributeOnFieldWarning,
                        fieldSymbol,
                        fieldSymbol.ContainingType,
                        fieldSymbol.Name);
                }

                // If the containing type is valid, track it
                if (fieldSymbol.ContainingType.InheritsFromFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservableValidator"))
                {
                    isValidationTargetValid = true;

                    return true;
                }

                // Otherwise just emit the diagnostic and then ignore the attribute
                diagnostics.Add(
                    MissingObservableValidatorInheritanceForNotifyDataErrorInfoError,
                    fieldSymbol,
                    fieldSymbol.ContainingType,
                    fieldSymbol.Name);

                isValidationTargetValid = false;

                return true;
            }

            isValidationTargetValid = false;

            return false;
        }

        /// <summary>
        /// Checks whether the generated code has to directly reference the old property value.
        /// </summary>
        /// <param name="fieldSymbol">The input <see cref="IFieldSymbol"/> instance to process.</param>
        /// <param name="propertyName">The name of the property being generated.</param>
        /// <returns>Whether the generated code needs direct access to the old property value.</returns>
        private static bool IsOldPropertyValueDirectlyReferenced(IFieldSymbol fieldSymbol, string propertyName)
        {
            // Check On<PROPERTY_NAME>Changing(<PROPERTY_TYPE> oldValue, <PROPERTY_TYPE> newValue) first
            foreach (ISymbol symbol in fieldSymbol.ContainingType.GetMembers($"On{propertyName}Changing"))
            {
                // No need to be too specific as we're not expecting false positives (which also wouldn't really
                // cause any problems anyway, just produce slightly worse codegen). Just checking the number of
                // parameters is good enough, and keeps the code very simple and cheap to run.
                if (symbol is IMethodSymbol { Parameters.Length: 2 })
                {
                    return true;
                }
            }

            // Do the same for On<PROPERTY_NAME>Changed(<PROPERTY_TYPE> oldValue, <PROPERTY_TYPE> newValue)
            foreach (ISymbol symbol in fieldSymbol.ContainingType.GetMembers($"On{propertyName}Changed"))
            {
                if (symbol is IMethodSymbol { Parameters.Length: 2 })
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets a <see cref="CompilationUnitSyntax"/> instance with the cached args for property changing notifications.
        /// </summary>
        /// <param name="names">The sequence of property names to cache args for.</param>
        /// <returns>A <see cref="CompilationUnitSyntax"/> instance with the sequence of cached args, if any.</returns>
        public static CompilationUnitSyntax? GetKnownPropertyChangingArgsSyntax(ImmutableArray<string> names)
        {
            return GetKnownPropertyChangingOrChangedArgsSyntax(
                "__KnownINotifyPropertyChangingArgs",
                "global::System.ComponentModel.PropertyChangingEventArgs",
                names);
        }

        /// <summary>
        /// Gets a <see cref="CompilationUnitSyntax"/> instance with the cached args for property changed notifications.
        /// </summary>
        /// <param name="names">The sequence of property names to cache args for.</param>
        /// <returns>A <see cref="CompilationUnitSyntax"/> instance with the sequence of cached args, if any.</returns>
        public static CompilationUnitSyntax? GetKnownPropertyChangedArgsSyntax(ImmutableArray<string> names)
        {
            return GetKnownPropertyChangingOrChangedArgsSyntax(
                "__KnownINotifyPropertyChangedArgs",
                "global::System.ComponentModel.PropertyChangedEventArgs",
                names);
        }

        /// <summary>
        /// Gets the <see cref="MemberDeclarationSyntax"/> instance for the input field.
        /// </summary>
        /// <param name="propertyInfo">The input <see cref="PropertyInfo"/> instance to process.</param>
        /// <returns>The generated <see cref="MemberDeclarationSyntax"/> instance for <paramref name="propertyInfo"/>.</returns>
        public static MemberDeclarationSyntax GetPropertySyntax(PropertyInfo propertyInfo)
        {
            using ImmutableArrayBuilder<StatementSyntax> setterStatements = ImmutableArrayBuilder<StatementSyntax>.Rent();

            // Get the property type syntax
            TypeSyntax propertyType = IdentifierName(propertyInfo.TypeNameWithNullabilityAnnotations);

            // In case the backing field is exactly named "value", we need to add the "this." prefix to ensure that comparisons and assignments
            // with it in the generated setter body are executed correctly and without conflicts with the implicit value parameter.
            ExpressionSyntax fieldExpression = propertyInfo.FieldName switch
            {
                "value" => MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, ThisExpression(), IdentifierName("value")),
                string name => IdentifierName(name)
            };

            if (propertyInfo.NotifyPropertyChangedRecipients || propertyInfo.IsOldPropertyValueDirectlyReferenced)
            {
                // Store the old value for later. This code generates a statement as follows:
                //
                // <PROPERTY_TYPE> __oldValue = <FIELD_EXPRESSIONS>;
                setterStatements.Add(
                    LocalDeclarationStatement(
                        VariableDeclaration(propertyType)
                        .AddVariables(
                            VariableDeclarator(Identifier("__oldValue"))
                            .WithInitializer(EqualsValueClause(fieldExpression)))));
            }

            // Add the OnPropertyChanging() call first:
            //
            // On<PROPERTY_NAME>Changing(value);
            setterStatements.Add(
                ExpressionStatement(
                    InvocationExpression(IdentifierName($"On{propertyInfo.PropertyName}Changing"))
                    .AddArgumentListArguments(Argument(IdentifierName("value")))));

            // Optimization: if the previous property value is not being referenced (which we can check by looking for an existing
            // symbol matching the name of either of these generated methods), we can pass a default expression and avoid generating
            // a field read, which won't otherwise be elided by Roslyn. Otherwise, we just store the value in a local as usual.
            ArgumentSyntax oldPropertyValueArgument = propertyInfo.IsOldPropertyValueDirectlyReferenced switch
            {
                true => Argument(IdentifierName("__oldValue")),
                false => Argument(LiteralExpression(SyntaxKind.DefaultLiteralExpression, Token(SyntaxKind.DefaultKeyword)))
            };

            // Also call the overload after that:
            //
            // On<PROPERTY_NAME>Changing(<OLD_PROPERTY_VALUE_EXPRESSION>, value);
            setterStatements.Add(
                ExpressionStatement(
                    InvocationExpression(IdentifierName($"On{propertyInfo.PropertyName}Changing"))
                    .AddArgumentListArguments(oldPropertyValueArgument, Argument(IdentifierName("value")))));

            // Gather the statements to notify dependent properties
            foreach (string propertyName in propertyInfo.PropertyChangingNames)
            {
                // This code generates a statement as follows:
                //
                // OnPropertyChanging(global::CommunityToolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangingArgs.<PROPERTY_NAME>);
                setterStatements.Add(
                    ExpressionStatement(
                        InvocationExpression(IdentifierName("OnPropertyChanging"))
                        .AddArgumentListArguments(Argument(MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("global::CommunityToolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangingArgs"),
                            IdentifierName(propertyName))))));
            }

            // Add the assignment statement:
            //
            // <FIELD_EXPRESSION> = value;
            setterStatements.Add(
                ExpressionStatement(
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        fieldExpression,
                        IdentifierName("value"))));

            // If validation is requested, add a call to ValidateProperty:
            //
            // ValidateProperty(value, <PROPERTY_NAME>);
            if (propertyInfo.NotifyDataErrorInfo)
            {
                setterStatements.Add(
                    ExpressionStatement(
                        InvocationExpression(IdentifierName("ValidateProperty"))
                        .AddArgumentListArguments(
                            Argument(IdentifierName("value")),
                            Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(propertyInfo.PropertyName))))));
            }

            // Add the OnPropertyChanged() call:
            //
            // On<PROPERTY_NAME>Changed(value);
            setterStatements.Add(
                ExpressionStatement(
                    InvocationExpression(IdentifierName($"On{propertyInfo.PropertyName}Changed"))
                    .AddArgumentListArguments(Argument(IdentifierName("value")))));

            // Do the same for the overload, as above:
            //
            // On<PROPERTY_NAME>Changed(<OLD_PROPERTY_VALUE_EXPRESSION>, value);
            setterStatements.Add(
                ExpressionStatement(
                    InvocationExpression(IdentifierName($"On{propertyInfo.PropertyName}Changed"))
                    .AddArgumentListArguments(oldPropertyValueArgument, Argument(IdentifierName("value")))));

            // Gather the statements to notify dependent properties
            foreach (string propertyName in propertyInfo.PropertyChangedNames)
            {
                // This code generates a statement as follows:
                //
                // OnPropertyChanging(global::CommunityToolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedArgs.<PROPERTY_NAME>);
                setterStatements.Add(
                    ExpressionStatement(
                        InvocationExpression(IdentifierName("OnPropertyChanged"))
                        .AddArgumentListArguments(Argument(MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("global::CommunityToolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedArgs"),
                            IdentifierName(propertyName))))));
            }

            // Gather the statements to notify commands
            foreach (string commandName in propertyInfo.NotifiedCommandNames)
            {
                // This code generates a statement as follows:
                //
                // <COMMAND_NAME>.NotifyCanExecuteChanged();
                setterStatements.Add(
                    ExpressionStatement(
                        InvocationExpression(MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(commandName),
                            IdentifierName("NotifyCanExecuteChanged")))));
            }

            // Also broadcast the change, if requested
            if (propertyInfo.NotifyPropertyChangedRecipients)
            {
                // This code generates a statement as follows:
                //
                // Broadcast(__oldValue, value, "<PROPERTY_NAME>");
                setterStatements.Add(
                    ExpressionStatement(
                        InvocationExpression(IdentifierName("Broadcast"))
                        .AddArgumentListArguments(
                            Argument(IdentifierName("__oldValue")),
                            Argument(IdentifierName("value")),
                            Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(propertyInfo.PropertyName))))));
            }

            // Generate the inner setter block as follows:
            //
            // if (!global::System.Collections.Generic.EqualityComparer<<PROPERTY_TYPE>>.Default.Equals(<FIELD_EXPRESSION>, value))
            // {
            //     <STATEMENTS>
            // }
            IfStatementSyntax setterIfStatement =
                IfStatement(
                    PrefixUnaryExpression(
                        SyntaxKind.LogicalNotExpression,
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    GenericName(Identifier("global::System.Collections.Generic.EqualityComparer"))
                                    .AddTypeArgumentListArguments(propertyType),
                                    IdentifierName("Default")),
                                IdentifierName("Equals")))
                        .AddArgumentListArguments(
                            Argument(fieldExpression),
                            Argument(IdentifierName("value")))),
                    Block(setterStatements.ToArray()));

            // Prepare the forwarded attributes, if any
            ImmutableArray<AttributeListSyntax> forwardedAttributes =
                propertyInfo.ForwardedAttributes
                .Select(static a => AttributeList(SingletonSeparatedList(a.GetSyntax())))
                .ToImmutableArray();

            // Construct the generated property as follows:
            //
            // /// <inheritdoc cref="<FIELD_NAME>"/>
            // [global::System.CodeDom.Compiler.GeneratedCode("...", "...")]
            // [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            // <FORWARDED_ATTRIBUTES>
            // public <FIELD_TYPE><NULLABLE_ANNOTATION?> <PROPERTY_NAME>
            // {
            //     get => <FIELD_NAME>;
            //     set
            //     {
            //         <BODY>
            //     }
            // }
            return
                PropertyDeclaration(propertyType, Identifier(propertyInfo.PropertyName))
                .AddAttributeLists(
                    AttributeList(SingletonSeparatedList(
                        Attribute(IdentifierName("global::System.CodeDom.Compiler.GeneratedCode"))
                        .AddArgumentListArguments(
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ObservablePropertyGenerator).FullName))),
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ObservablePropertyGenerator).Assembly.GetName().Version.ToString()))))))
                    .WithOpenBracketToken(Token(TriviaList(Comment($"/// <inheritdoc cref=\"{propertyInfo.FieldName}\"/>")), SyntaxKind.OpenBracketToken, TriviaList())),
                    AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage")))))
                .AddAttributeLists(forwardedAttributes.ToArray())
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .AddAccessorListAccessors(
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithExpressionBody(ArrowExpressionClause(IdentifierName(propertyInfo.FieldName)))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                    AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .WithBody(Block(setterIfStatement)));
        }

        /// <summary>
        /// Gets the <see cref="MemberDeclarationSyntax"/> instances for the <c>OnPropertyChanging</c> and <c>OnPropertyChanged</c> methods for the input field.
        /// </summary>
        /// <param name="propertyInfo">The input <see cref="PropertyInfo"/> instance to process.</param>
        /// <returns>The generated <see cref="MemberDeclarationSyntax"/> instances for the <c>OnPropertyChanging</c> and <c>OnPropertyChanged</c> methods.</returns>
        public static ImmutableArray<MemberDeclarationSyntax> GetOnPropertyChangeMethodsSyntax(PropertyInfo propertyInfo)
        {
            // Get the property type syntax
            TypeSyntax parameterType = IdentifierName(propertyInfo.TypeNameWithNullabilityAnnotations);

            // Construct the generated method as follows:
            //
            // /// <summary>Executes the logic for when <see cref="<PROPERTY_NAME>"/> is changing.</summary>
            // /// <param name="value">The new property value being set.</param>
            // /// <remarks>This method is invoked right before the value of <see cref="<PROPERTY_NAME>"/> is changed.</remarks>
            // [global::System.CodeDom.Compiler.GeneratedCode("...", "...")]
            // partial void On<PROPERTY_NAME>Changing(<PROPERTY_TYPE> value);
            MemberDeclarationSyntax onPropertyChangingDeclaration =
                MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier($"On{propertyInfo.PropertyName}Changing"))
                .AddModifiers(Token(SyntaxKind.PartialKeyword))
                .AddParameterListParameters(Parameter(Identifier("value")).WithType(parameterType))
                .AddAttributeLists(
                    AttributeList(SingletonSeparatedList(
                        Attribute(IdentifierName("global::System.CodeDom.Compiler.GeneratedCode"))
                        .AddArgumentListArguments(
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ObservablePropertyGenerator).FullName))),
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ObservablePropertyGenerator).Assembly.GetName().Version.ToString()))))))
                    .WithOpenBracketToken(Token(TriviaList(
                        Comment($"/// <summary>Executes the logic for when <see cref=\"{propertyInfo.PropertyName}\"/> is changing.</summary>"),
                        Comment("/// <param name=\"value\">The new property value being set.</param>"),
                        Comment($"/// <remarks>This method is invoked right before the value of <see cref=\"{propertyInfo.PropertyName}\"/> is changed.</remarks>")), SyntaxKind.OpenBracketToken, TriviaList())))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            // Prepare the nullable type for the previous property value. This is needed because if the type is a reference
            // type, the previous value might be null even if the property type is not nullable, as the first invocation would
            // happen when the property is first set to some value that is not null (but the backing field would still be so).
            // As a cheap way to check whether we need to add nullable, we can simply check whether the type name with nullability
            // annotations ends with a '?'. If it doesn't and the type is a reference type, we add it. Otherwise, we keep it.
            TypeSyntax oldValueTypeSyntax = propertyInfo.IsReferenceType switch
            {
                true when !propertyInfo.TypeNameWithNullabilityAnnotations.EndsWith("?")
                    => IdentifierName($"{propertyInfo.TypeNameWithNullabilityAnnotations}?"),
                _ => parameterType
            };

            // Construct the generated method as follows:
            //
            // /// <summary>Executes the logic for when <see cref="<PROPERTY_NAME>"/> is changing.</summary>
            // /// <param name="oldValue">The previous property value that is being replaced.</param>
            // /// <param name="newValue">The new property value being set.</param>
            // /// <remarks>This method is invoked right before the value of <see cref="<PROPERTY_NAME>"/> is changed.</remarks>
            // [global::System.CodeDom.Compiler.GeneratedCode("...", "...")]
            // partial void On<PROPERTY_NAME>Changing(<OLD_VALUE_TYPE> oldValue, <PROPERTY_TYPE> newValue);
            MemberDeclarationSyntax onPropertyChanging2Declaration =
                MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier($"On{propertyInfo.PropertyName}Changing"))
                .AddModifiers(Token(SyntaxKind.PartialKeyword))
                .AddParameterListParameters(
                    Parameter(Identifier("oldValue")).WithType(oldValueTypeSyntax),
                    Parameter(Identifier("newValue")).WithType(parameterType))
                .AddAttributeLists(
                    AttributeList(SingletonSeparatedList(
                        Attribute(IdentifierName("global::System.CodeDom.Compiler.GeneratedCode"))
                        .AddArgumentListArguments(
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ObservablePropertyGenerator).FullName))),
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ObservablePropertyGenerator).Assembly.GetName().Version.ToString()))))))
                    .WithOpenBracketToken(Token(TriviaList(
                        Comment($"/// <summary>Executes the logic for when <see cref=\"{propertyInfo.PropertyName}\"/> is changing.</summary>"),
                        Comment("/// <param name=\"oldValue\">The previous property value that is being replaced.</param>"),
                        Comment("/// <param name=\"newValue\">The new property value being set.</param>"),
                        Comment($"/// <remarks>This method is invoked right before the value of <see cref=\"{propertyInfo.PropertyName}\"/> is changed.</remarks>")), SyntaxKind.OpenBracketToken, TriviaList())))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            // Construct the generated method as follows:
            //
            // /// <summary>Executes the logic for when <see cref="<PROPERTY_NAME>"/> ust changed.</summary>
            // /// <param name="value">The new property value that was set.</param>
            // /// <remarks>This method is invoked right after the value of <see cref="<PROPERTY_NAME>"/> is changed.</remarks>
            // [global::System.CodeDom.Compiler.GeneratedCode("...", "...")]
            // partial void On<PROPERTY_NAME>Changed(<PROPERTY_TYPE> value);
            MemberDeclarationSyntax onPropertyChangedDeclaration =
                MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier($"On{propertyInfo.PropertyName}Changed"))
                .AddModifiers(Token(SyntaxKind.PartialKeyword))
                .AddParameterListParameters(Parameter(Identifier("value")).WithType(parameterType))
                .AddAttributeLists(
                    AttributeList(SingletonSeparatedList(
                        Attribute(IdentifierName("global::System.CodeDom.Compiler.GeneratedCode"))
                        .AddArgumentListArguments(
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ObservablePropertyGenerator).FullName))),
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ObservablePropertyGenerator).Assembly.GetName().Version.ToString()))))))
                    .WithOpenBracketToken(Token(TriviaList(
                        Comment($"/// <summary>Executes the logic for when <see cref=\"{propertyInfo.PropertyName}\"/> just changed.</summary>"),
                        Comment("/// <param name=\"value\">The new property value that was set.</param>"),
                        Comment($"/// <remarks>This method is invoked right after the value of <see cref=\"{propertyInfo.PropertyName}\"/> is changed.</remarks>")), SyntaxKind.OpenBracketToken, TriviaList())))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            // Construct the generated method as follows:
            //
            // /// <summary>Executes the logic for when <see cref="<PROPERTY_NAME>"/> ust changed.</summary>
            // /// <param name="oldValue">The previous property value that was replaced.</param>
            // /// <param name="newValue">The new property value that was set.</param>
            // /// <remarks>This method is invoked right after the value of <see cref="<PROPERTY_NAME>"/> is changed.</remarks>
            // [global::System.CodeDom.Compiler.GeneratedCode("...", "...")]
            // partial void On<PROPERTY_NAME>Changed(<OLD_VALUE_TYPE> oldValue, <PROPERTY_TYPE> newValue);
            MemberDeclarationSyntax onPropertyChanged2Declaration =
                MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier($"On{propertyInfo.PropertyName}Changed"))
                .AddModifiers(Token(SyntaxKind.PartialKeyword))
                .AddParameterListParameters(
                    Parameter(Identifier("oldValue")).WithType(oldValueTypeSyntax),
                    Parameter(Identifier("newValue")).WithType(parameterType))
                .AddAttributeLists(
                    AttributeList(SingletonSeparatedList(
                        Attribute(IdentifierName("global::System.CodeDom.Compiler.GeneratedCode"))
                        .AddArgumentListArguments(
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ObservablePropertyGenerator).FullName))),
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ObservablePropertyGenerator).Assembly.GetName().Version.ToString()))))))
                    .WithOpenBracketToken(Token(TriviaList(
                        Comment($"/// <summary>Executes the logic for when <see cref=\"{propertyInfo.PropertyName}\"/> just changed.</summary>"),
                        Comment("/// <param name=\"oldValue\">The previous property value that was replaced.</param>"),
                        Comment("/// <param name=\"newValue\">The new property value that was set.</param>"),
                        Comment($"/// <remarks>This method is invoked right after the value of <see cref=\"{propertyInfo.PropertyName}\"/> is changed.</remarks>")), SyntaxKind.OpenBracketToken, TriviaList())))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            return ImmutableArray.Create(
                onPropertyChangingDeclaration,
                onPropertyChanging2Declaration,
                onPropertyChangedDeclaration,
                onPropertyChanged2Declaration);
        }

        /// <summary>
        /// Gets a <see cref="CompilationUnitSyntax"/> instance with the cached args of a specified type.
        /// </summary>
        /// <param name="ContainingTypeName">The name of the generated type.</param>
        /// <param name="ArgsTypeName">The argument type name.</param>
        /// <param name="names">The sequence of property names to cache args for.</param>
        /// <returns>A <see cref="CompilationUnitSyntax"/> instance with the sequence of cached args, if any.</returns>
        private static CompilationUnitSyntax? GetKnownPropertyChangingOrChangedArgsSyntax(
            string ContainingTypeName,
            string ArgsTypeName,
            ImmutableArray<string> names)
        {
            if (names.IsEmpty)
            {
                return null;
            }

            // This code takes a class symbol and produces a compilation unit as follows:
            //
            // // <auto-generated/>
            // #pragma warning disable
            // #nullable enable
            // namespace CommunityToolkit.Mvvm.ComponentModel.__Internals
            // {
            //     [global::System.CodeDom.Compiler.GeneratedCode("...", "...")]
            //     [global::System.Diagnostics.DebuggerNonUserCode]
            //     [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            //     [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
            //     [global::System.Obsolete("This type is not intended to be used directly by user code")]
            //     internal static class <CONTAINING_TYPE_NAME>
            //     {
            //         <FIELDS>
            //     }
            // }
            return
                CompilationUnit().AddMembers(
                NamespaceDeclaration(IdentifierName("CommunityToolkit.Mvvm.ComponentModel.__Internals")).WithLeadingTrivia(TriviaList(
                    Comment("// <auto-generated/>"),
                    Trivia(PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), true)),
                    Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)))).AddMembers(
                ClassDeclaration(ContainingTypeName).AddModifiers(
                    Token(SyntaxKind.InternalKeyword),
                    Token(SyntaxKind.StaticKeyword)).AddAttributeLists(
                        AttributeList(SingletonSeparatedList(
                            Attribute(IdentifierName($"global::System.CodeDom.Compiler.GeneratedCode"))
                            .AddArgumentListArguments(
                                AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ObservablePropertyGenerator).FullName))),
                                AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ObservablePropertyGenerator).Assembly.GetName().Version.ToString())))))),
                        AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.DebuggerNonUserCode")))),
                        AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage")))),
                        AttributeList(SingletonSeparatedList(
                            Attribute(IdentifierName("global::System.ComponentModel.EditorBrowsable")).AddArgumentListArguments(
                            AttributeArgument(ParseExpression("global::System.ComponentModel.EditorBrowsableState.Never"))))),
                        AttributeList(SingletonSeparatedList(
                            Attribute(IdentifierName("global::System.Obsolete")).AddArgumentListArguments(
                            AttributeArgument(LiteralExpression(
                                SyntaxKind.StringLiteralExpression,
                                Literal("This type is not intended to be used directly by user code")))))))
                    .AddMembers(names.Select(name => CreateFieldDeclaration(ArgsTypeName, name)).ToArray())))
                .NormalizeWhitespace();
        }

        /// <summary>
        /// Creates a field declaration for a cached property changing/changed name.
        /// </summary>
        /// <param name="typeName">The field type name (either <see cref="PropertyChangedEventArgs"/> or <see cref="PropertyChangingEventArgs"/>).</param>
        /// <param name="propertyName">The name of the cached property name.</param>
        /// <returns>A <see cref="FieldDeclarationSyntax"/> instance for the input cached property name.</returns>
        private static FieldDeclarationSyntax CreateFieldDeclaration(string typeName, string propertyName)
        {
            // Create a static field with a cached property changed/changing argument for a specified property.
            // This code produces a field declaration as follows:
            //
            // [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
            // [global::System.Obsolete("This field is not intended to be referenced directly by user code")]
            // public static readonly <ARG_TYPE> <PROPERTY_NAME> = new("<PROPERTY_NAME>");
            return
                FieldDeclaration(
                VariableDeclaration(IdentifierName(typeName))
                .AddVariables(
                    VariableDeclarator(Identifier(propertyName))
                    .WithInitializer(EqualsValueClause(
                        ObjectCreationExpression(IdentifierName(typeName))
                        .AddArgumentListArguments(Argument(
                            LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(propertyName))))))))
                .AddModifiers(
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.StaticKeyword),
                    Token(SyntaxKind.ReadOnlyKeyword))
                .AddAttributeLists(
                    AttributeList(SingletonSeparatedList(
                        Attribute(IdentifierName("global::System.ComponentModel.EditorBrowsable")).AddArgumentListArguments(
                        AttributeArgument(ParseExpression("global::System.ComponentModel.EditorBrowsableState.Never"))))),
                    AttributeList(SingletonSeparatedList(
                        Attribute(IdentifierName("global::System.Obsolete")).AddArgumentListArguments(
                        AttributeArgument(LiteralExpression(
                            SyntaxKind.StringLiteralExpression,
                            Literal("This field is not intended to be referenced directly by user code")))))));
        }

        /// <summary>
        /// Get the generated property name for an input field.
        /// </summary>
        /// <param name="fieldSymbol">The input <see cref="IFieldSymbol"/> instance to process.</param>
        /// <returns>The generated property name for <paramref name="fieldSymbol"/>.</returns>
        public static string GetGeneratedPropertyName(IFieldSymbol fieldSymbol)
        {
            string propertyName = fieldSymbol.Name;

            if (propertyName.StartsWith("m_"))
            {
                propertyName = propertyName.Substring(2);
            }
            else if (propertyName.StartsWith("_"))
            {
                propertyName = propertyName.TrimStart('_');
            }

            return $"{char.ToUpper(propertyName[0], CultureInfo.InvariantCulture)}{propertyName.Substring(1)}";
        }
    }
}
