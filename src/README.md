## Development information

### 🧱 Structure
```
 📂src
 ┣ 📂back-end
 ┃ ┣ 📂shared
 ┃ ┃  ┗ 📦 Nuget Packages (.NET 7)
 ┃ ┗ 📡 API / GRPC Projects
 ┣ 📂front-end
 ┃ ┣ 📂shared
 ┃ ┃  ┗ 📦 Nuget Packages (.NET 7)
 ┃ ┗ 🖼️ Front-end themes (Blazor .NET)
 ┣ 📂smart-contracts
 ┃ ┣ 📂nft-contracts
 ┃ ┃ ┗ 📜 Flower.sol (Solidity)
 ┃ ┣ 📂exchange
 ┃ ┃ ┗ 📜 GRDNToken.sol (Solidity)
 ┣ 📂landing-page
 ┃ ┗ 🔗 Official Site (Blazor WebAssembly, PWA)
 ┣ 📂nft-market
 ┃ ┗ ⛓️ NFT Store Web3 DApp (Blazor WebAssembly, PWA)
 ┣ 📂charity-page
 ┃ ┗ ⛓️ Charity Web3 DApp (Blazor WebAssembly, PWA)
 ┣ 📂exchange
 ┃ ┗ ⛓️ Exchange Web3 DApp (Blazor WebAssembly, PWA)
 ┣ 📂metaverse
 ┃ ┣ 🔗 Metaverse Site (Blazor WebAssembly, PWA)
 ┃ ┗ 🌐 Metaverse (Unity C#)
 ┗ 📂mobile-apps
   ┣ 🔗 Mobile Application Site (Blazor WebAssembly, PWA)
   ┗ 📱 Mobile Applications, Android, IOS, Windows  (.NET Multi-platform App UI)
```
### 📦 Packages
### 🖼️ Nuget Packages - Shared Frond-end Blazor components  📦

| Package | Type | Registry | Link | Status |
| - | - | - | - | - | 
| Moralis Metamask Components |  Nuget | Github Package Registry | [Latest](https://coingarden.world) | |

---
### 📡 Nuget Packages - Shared Back-end Azure Functions  📦

| Package | Type | Registry | Link | Status |
| - | - | - | - | - | 
| Moralis Azure Functions | Nuget | Github Package Registry | [Latest](https://coingarden.world) | |

---

## Infrastructure 

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