# Contributing

We welcome contributions in several forms, e.g.

* Sponsoring
* Documenting
* Testing
* Coding
* etc.

Please check the [Issues](https://github.com/siemens/tia-portal-openness-code-snippets/issues) and look for
unassigned ones or create a new one.

Working together in an open and welcoming environment is the foundation of our
success, so please respect our [Code of Conduct](CODE_OF_CONDUCT.md).

You can contribute even if you are new to the platform. The article below lays out ways you can make meaningful
contributions regardless of your skill levels. It remains highly relevant and also applies to our Inner Source approach:

[14 Ways to Contribute to Open Source without Being a Programming Genius or a Rock Star](https://web.archive.org/web/20231130050456/https://smartbear.com/blog/14-ways-to-contribute-to-open-source-without-being/).

## Guidelines

### Workflow

We use the
[Feature Branch Workflow](https://www.atlassian.com/git/tutorials/comparing-workflows/feature-branch-workflow)
and review all changes we merge to the main branch.

We appreciate any contributions, so please use the [Forking Workflow](https://www.atlassian.com/git/tutorials/comparing-workflows/forking-workflow)
and send us [Merge Requests](https://docs.gitlab.com/17.10/user/project/merge_requests/)!

### Commit Message

Commit messages shall follow the conventions defined by [conventionalcommits](https://www.conventionalcommits.org/en/v1.0.0/#summary), for example:

* `docs(security): add dockle security chapter`
* `fix(theme): properly size and position hero img`
* `style(markdown): run markdownlint auto-fix with latest version`

If you accidentally pushed a commit with a malformed message you have to [reword the commit](https://docs.github.com/en/pull-requests/committing-changes-to-your-project/creating-and-editing-commits/changing-a-commit-message).

#### What To Use as Scope

In most cases the changed component is a good choice as scope
e.g. if the change is done in the windows manual, the scope should be *windows*.

For documentation changes the section that was changed makes a good scope name
e.g. use *FAQ* if you changed that section.