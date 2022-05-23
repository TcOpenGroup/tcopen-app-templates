# !!!WORK IN PROGRESS!!!

# TcOpen-app-templates

This repository contains templates for TcOpen applications.

## Scaffolding from template

1. Download the script *scaffold-from-template.ps1* and copy it to an empty folder.
1. Run *scaffold-from-template.ps1* script in an empty folder.

Example

~~~PS1
.\scaffold-from-template.ps1 -Branch dev -ProjectName MyFirstProject -TemplateId mts-s-template
~~~

|  Argument   |                                                                         Description                                                                          |
| ----------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Branch      | Indicates from which branch of this repo the template should be taken (dev, master, etc.)                                                                    |
| ProjectName | Name of the project. The scaffolding will rename template variables with this name (arbitrary name no spaces and project name with name 't' is not allowed). |
| TemplateId  | Id of the template from which we want scaffold the project (see table bellow.                                                                                |

## Available project templates

|    Template id    |                                           Description                                            |
| ----------------- | ------------------------------------------------------------------------------------------------ |
| mts-s-template    | Standard template for [MTS](https://www.mts.sk/en/) more in the readme of the template  `templates\mts-s-template` |
| more coming soon | -                                                                                                |


