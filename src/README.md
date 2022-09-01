# Infrastructure 

### Splitted to folders with the idea, that if this repository became to big, it will be splitted to different submodules (check picture bellow how to checkout the code when that happens)
### All builds and deployments will be splitted too and when changes are made to some submodule it will be the only one build and deployed 
---
### Folder Structure
- ci-cd  
  - Folder with DevOps sripts, builds and deployment scripts
- back-end 
  - Contains the APIs and the main C# code 
- frond-end 
  - used for the main site designs and scripts

### How to checkout git submodules

![recurse-submodules](https://github.com/s2kdesign-com/CoinGarden-World-Full/raw/main/docs/assets/recurse-submodules.png)