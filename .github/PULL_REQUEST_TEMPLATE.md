<!-- 🚨 Please Do Not skip any instructions and information mentioned below as they are all required and essential to evaluate and test the PR. By fulfilling all the required information you will be able to reduce the volume of questions and most likely help merge the PR faster 🚨 -->

<!-- ⚠️ We will not merge the PR to the main repo if your changes are not in a *feature branch* of your forked repository. Create a new branch in your repo, **do not use the `main` branch in your repo**! -->

<!-- ⚠️ **Every PR** needs to have a linked issue and have previously been approved. PRs that don't follow this will be rejected. >

<!-- 👉 It is imperative to resolve ONE ISSUE PER PR and avoid making multiple changes unless the changes interrelate with each other -->

<!-- 📝 Please always keep the "☑️ Allow edits by maintainers" button checked in the Pull Request Template as it increases collaboration with the Toolkit maintainers by permitting commits to your PR branch (only) created from your fork. This can let us quickly make fixes for minor typos or forgotten StyleCop issues during review without needing to wait on you doing extra work. Let us help you help us! 🎉 -->

**Closes #<ISSUE_NUMBER>**

<!-- Add an overview of the changes here -->

<!-- All details should be in the linked issue. Feel free to call out any outstanding differences here. -->

## PR Checklist

<!-- Please check if your PR fulfills the following requirements, and remove the ones that are not applicable to the current PR -->

- [ ] Created a feature/dev branch in your fork (vs. submitting directly from a commit on main)
- [ ] Based off latest main branch of toolkit
- [ ] PR doesn't include merge commits (always rebase on top of our main, if needed)
- [ ] Tested code with current [supported SDKs](../#supported)
- [ ] New component
  - [ ] Pull Request has been submitted to the documentation repository [instructions](../blob/main/Contributing.md#docs). Link: <!-- docs PR link -->
  - [ ] Added description of major feature to project description for NuGet package (4000 total character limit, so don't push entire description over that)
- [ ] Tests for the changes have been added (for bug fixes / features) (if applicable)
- [ ] Header has been added to all new source files (run _build/UpdateHeaders.bat_)
- [ ] Contains **NO** breaking changes
- [ ] Every new API (including internal ones) has full XML docs
- [ ] Code follows all style conventions

<!-- If this PR contains a breaking change, please describe the impact and migration path for existing applications below.
Please note that breaking changes are likely to be rejected within minor release cycles or held until major versions. -->

## Other information

<!-- Please add any other information that might be helpful to reviewers. -->