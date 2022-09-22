// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

export async function checkMetaMask() {
    // Modern dapp browsers...
    if (window.ethereum) {
        if (ethereum.selectedAddress === null || ethereum.selectedAddress === undefined) {
            try {
                // TODO: do not request account login again , the user can press the button for login
                // Request account access if needed
                await requestAccounts();
            } catch (error) {
                // User denied account access...
                console.log("UserDenied");
                //throw "UserDenied"
            }
        }
        else {
            console.log("Selected:" + ethereum.selectedAddress);
        }
    }
    // Non-dapp browsers...
    else {
        throw "NoMetaMask"
    }
}


export async function signTypedData(label, value) {
    await checkMetaMask();

    const msgParams = [
        {
            type: 'string', // Valid solidity type
            name: label,
            value: value
        }
    ]

    try {
        var result = await ethereum.request({
            method: 'eth_signTypedData',
            params:
            [
                msgParams,
                ethereum.selectedAddress
            ]
        });

        return result;
    } catch (error) {
        // User denied account access...
        throw "UserDenied"
    }
}

export async function signTypedDataV4(typedData) {
    await checkMetaMask();

    try {
        var result = await ethereum.request({
            method: 'eth_signTypedData_v4',
            params:
            [
                ethereum.selectedAddress,
                typedData
            ],
            from: ethereum.selectedAddress
        });

        return result;
    } catch (error) {
        // User denied account access...
        throw "UserDenied"
    }
}

export async function requestAccounts() {
    console.log('reqAccount');
    var result = await ethereum.request({
        method: 'eth_requestAccounts',
    });

    return result;
}

export function hasMetaMask() {
    return (window.ethereum != undefined);
}

export function isSiteConnected() {
    return (window.ethereum != undefined && (ethereum.selectedAddress != undefined || ethereum.selectedAddress != null));
}

export async function getSelectedAddress() {
    await checkMetaMask();

    return ethereum.selectedAddress;
}

// web3 logout function
export async function logout() {
    // set the global ethereum.selectedAddress variable to null
    ethereum.selectedAddresss = null;

    // remove the user's wallet address from local storage
   // window.localStorage.removeItem("userWalletAddress");

};

export async function listenToChangeEvents() {
    if (hasMetaMask()) {
        //ethereum.on("connect", function () {
        //    DotNet.invokeMethodAsync('MetaMask.Blazor', 'OnConnect');
        //});

        //ethereum.on("disconnect", function () {
        //    DotNet.invokeMethodAsync('MetaMask.Blazor', 'OnDisconnect');
        //});

        ethereum.on("accountsChanged", function (accounts) {
            DotNet.invokeMethodAsync('CoinGardenWorld.Moralis.Metamask', 'OnAccountsChanged', accounts[0]);
        });

        ethereum.on("chainChanged", function (chainId) {
            DotNet.invokeMethodAsync('CoinGardenWorld.Moralis.Metamask', 'OnChainChanged', chainId);
        });
    }
}

export async function getSelectedChain() {
    await checkMetaMask();

    var result = await ethereum.request({
        method: 'eth_chainId'
    });
    return result;
}

export async function getTransactionCount() {
    await checkMetaMask();

    var result = await ethereum.request({
        method: 'eth_getTransactionCount',
        params:
            [
                ethereum.selectedAddress,
                'latest'
            ]

    });
    return result;
}

export async function sendTransaction(to, value, data) {
    await checkMetaMask();

    const transactionParameters = {
        to: to,
        from: ethereum.selectedAddress, // must match user's active address.
        value: value,
        data: data
    };

    try {
        var result = await ethereum.request({
            method: 'eth_sendTransaction',
            params: [transactionParameters]
        });

        return result;
    } catch (error) {
        if (error.code == 4001) {
            throw "UserDenied"
        }
        throw error;
    }
}
export async function genericRpc(method, params) {
    await checkMetaMask();
    console.log(params)
    var result = await ethereum.request({
        method: method,
        params: params
    });
    return result;
}

// AJAX call
const xhr = (method, url, data) => {
    return new Promise(function (resolve, reject) {
        var xhr = new XMLHttpRequest();

        xhr.open(method, url);

        // Set CORS headers
        xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        xhr.setRequestHeader('Access-Control-Allow-Origin', '*');

        // Create a response listener
        xhr.onload = function () {
            if (this.status >= 200 && this.status < 300) {
                // Handle good response
                resolve(parse(xhr.response));
            } else {
                // Handle error response
                reject({
                    status: this.status,
                    statusText: xhr.statusText
                });
            }
        };

        // Create an error listener
        xhr.onerror = function () {
            reject({
                status: this.status,
                statusText: xhr.statusText
            });
        };

        // Send the request
        if (method == "POST" && data) {
            xhr.setRequestHeader('Content-type', 'application/json');
            xhr.send(data);
        } else {
            xhr.send();
        }
    });
}

// Parse ajax call from json to object.
const parse = (text) => {
    try {
        return JSON.parse(text);
    } catch (e) {
        return text;
    }
}
