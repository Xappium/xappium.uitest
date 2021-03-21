# Contributing

Thank you for your interest in contributing to Xappium UITest! In this document, we'll outline what you need to know about contributing and how to get started.

## Code of Conduct

Please see our [Code of Conduct](CODE_OF_CONDUCT.md).

<!-- ## Prerequisite

You will need to complete a Contribution License Agreement before any pull request can be accepted. Review the CLA at https://cla.dotnetfoundation.org/. When you submit a pull request, a CLA assistant bot will confirm you have completed the agreement, or provide you with an opportunity to do so. -->

## Contributing Code

Currently, we are in the beginning phases of building up Xappium UITest. Yet, we are still very excited for you to join us during this exciting time :)

### What to work on

If you're looking for something to work on, please browse our [Open Issues](https://github.com/xappium/xappium.uitest/issues). Any issue that is not already assigned is up for grabs.

Follow the style used by the [.NET Foundation](https://github.com/dotnet/runtime/blob/master/docs/coding-guidelines/coding-style.md), with the following additions:

- We do prefer new language features - for example `if (someVar is null)` over `if (someVar == null)`
- We do place a space between control structures and the opening parenthesis - for example `foreach (` instead of `foreach(` or `if (` instead of `if(`

Read and follow our [Pull Request template](PULL_REQUEST_TEMPLATE.md).

### Pull Request Requirements

Other than small PRs to fix typos, Pull Requests must originate from an open issue. Unsolicited PR's will be closed.

## Proposals/Enhancements/Suggestions

To propose a change or new feature, open an issue using the [Feature request template](https://github.com/xappium/xappium.uitest/issues/new?assignees=&labels=proposal-open%2C+t%2Fenhancement+➕&template=feature_request.md&title=[Enhancement]+YOUR+IDEA!). You may also use the [Spec template](https://github.com/xappium/xappium.uitest/issues/new?assignees=&labels=proposal-open%2C+t%2Fenhancement+➕&template=spec.md&title=[Spec]++) if you have an idea of what the API should look like. Be sure to also browse current issues and [discussions](https://github.com/xappium/xappium.uitest/discussions) that may be related to what you have in mind.

## Review Process
PR's generally will need to build and pass CI Testing. We will do our best to review pull requests in a timely manner, but please be patient. This project is Open Source and we do not currently get paid to do this, so we will get to it as our schedules allow. If there are any changes requested, the contributor should make them at their earliest convenience or let the reviewers know that they are unable to make further contributions. If the pull request requires only minor changes, then someone else may pick it up and finish it. We will do our best to make sure that all credit is retained for contributors. 

Once a pull request has an "approved" label, as long as no UI or unit tests are failing, this pull request can be merged at this time.

## Merge Process
PRs should target the master branch.