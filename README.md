# TcOpen application templates

This repository contains templates for TcOpen applications, for you to get started quickly, with the development and deployment of your TcOpen applications.

## Scaffolding from template

1. Go to the releases in this repository [here](https://github.com/TcOpenGroup/tcopen-app-templates/releases).
1. Open the latest release. 
1. From **Assets** Download the latest version of *TcOpen.Scaffold.UI.zip*, copy it to an empty folder and unzip.
1. Run *TcOpen.Scaffold.UI.exe*. 

> **You may get a security warning about the executing file download from the internet. We will provide singed binaries later in the development process. If you have reason to distrust this binary, you can compile your version from the source of this repository.**


![](assets\pics\scaffolder-ui.png)


You can use `CLI` version of the scaffolder. Download the `tco.cli.zip` asset from the latest release and run `tco.exe --help` to see the available options.

~~~
PS C:\> .\tco.exe --help
Running version: 0.1.1-alpha.35
tco 0.1.1-alpha.35+Branch.dev.Sha.c0ae152a3de0c0494209f94b520d966f63caecee
Copyright (C) 2022 author

  -b, --branch-tag          Branch from which draw the scaffold.

  -n, --project-name        (Default: MyProject) Project name.

  -n, --template-name       (Default: mts-s-template) Name of the template from which the project will be scaffolded.

  -o, --output-directory    Target directory for the scaffold.

  -r, --release             Release name.

  -s, --source              (Default: release) Source release or repository

  --help                    Display this help screen.

  --version                 Display version information.

~~~

## Available project templates

|    Template id    |                                           Description                                            |
| ----------------- | ------------------------------------------------------------------------------------------------ |
| mts-s-template    | Standard template for [MTS](https://www.mts.sk/en/) more in the readme of the template  [templates\mts-s-template](templates/mts-s-template/t/README.md)|
| more coming soon | -                                                                                                |

## Pre-requisites

- [General pre-requisites TcOpen](https://github.com/TcOpenGroup/TcOpen#prerequisites)

For the specific requirements of a particular template see the readme of the template.




