# Monster Trading Card Game by David Zelenay
[![.NET Core](https://github.com/cellularegg/swe-mtcg/workflows/.NET%20Core/badge.svg)](https://github.com/cellularegg/swe-mtcg/actions?query=workflow%3A%22.NET+Core%22)

Time Tracking:
| Date  | Time in h | Comment |
| ------------- | ------------- | ------------- |
| 15-oct-2020 | 3 | Setup Github repo, initial OOP class design (TDD) |
| 17-oct-2020 | 0.5 | Setup Github Actions |
| 18-oct-2020 | 1.5 | Further developed CardDeck Class |
|HTTP Webservice|-|-|
| 20-oct-2020 | 2.75 | Started developing HTTP Server |
| 21-oct-2020 | 3 | Added MessageCollection + Tests |
| 11-nov-2020 | 3 | Further developed MsgColl + Implemented GET /messages and POST /messages |
| 12-nov-2020 | 4 | Implemented other request Methods and msg in Path (GET /msgs/1) |
| 15-nov-2020 | 2 | Recorded Video + minor tweaks |
| 02-jan-2021 | 1.5 | Made HTTP Server Multithreaded |
| 02-jan-2021 | 3.5 | Read requirements thoroughly and redesigned class diagram |
| 03-jan-2021 | 7 | Developed Card Logic (GetAttackValue() + GetEffectivenessMultiplier() + CardCollection)  |
| 04-jan-2021 | x | Developed ServerData + Logic  |

## Class Diagram
![Class Diagram](https://raw.githubusercontent.com/cellularegg/swe-mtcg/dev/class_diagram.svg)
Note: Client / Curl Script is just a summary of the curl script to visualize the client requirements
## ToDo:
* Develop classes according to class diagram
* Think of a way to handle DB Connection / Persistence
* Write Protocol / Documentation
* Update Class Diagram


[Github Link](https://github.com/cellularegg/swe-mtcg)