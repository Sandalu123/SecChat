/**
 * ASP.NET Core SignalR JavaScript Client.
 * This is a simplified placeholder implementation for the SignalR client.
 * In a production environment, this would be replaced with the actual SignalR client library.
 */
(function (global, factory) {
    typeof exports === 'object' && typeof module !== 'undefined' ? factory(exports) :
    typeof define === 'function' && define.amd ? define(['exports'], factory) :
    (factory((global.signalR = {})));
}(this, (function (exports) {
    'use strict';

    // Simplified HubConnectionBuilder for demo purposes
    class HubConnectionBuilder {
        constructor() {
            this._url = null;
            this._logLevel = 0;
            this._reconnectPolicy = null;
        }

        withUrl(url) {
            this._url = url;
            return this;
        }

        configureLogging(logLevel) {
            this._logLevel = logLevel;
            return this;
        }

        withAutomaticReconnect(reconnectPolicy) {
            this._reconnectPolicy = reconnectPolicy || [0, 2000, 10000, 30000];
            return this;
        }

        build() {
            if (!this._url) {
                throw new Error("The 'withUrl' method must be called before building the connection.");
            }
            return new HubConnection(this._url, this._logLevel, this._reconnectPolicy);
        }
    }

    // Simplified HubConnection for demo purposes
    class HubConnection {
        constructor(url, logLevel, reconnectPolicy) {
            this.url = url;
            this.logLevel = logLevel;
            this.reconnectPolicy = reconnectPolicy;
            this.connectionState = "Disconnected";
            this.handlers = {};
            this.methods = {};

            console.log(`Hub connection created for ${url}`);
            console.log("This is a placeholder SignalR client implementation.");
            console.log("In production, use the actual @microsoft/signalr library.");
        }

        on(methodName, handler) {
            this.handlers[methodName] = this.handlers[methodName] || [];
            this.handlers[methodName].push(handler);
            return this;
        }

        invoke(methodName, ...args) {
            console.log(`Invoking method ${methodName} with arguments:`, args);
            return new Promise((resolve, reject) => {
                if (this.connectionState !== "Connected") {
                    reject(new Error("Cannot invoke method while disconnected."));
                    return;
                }
                
                // In a real implementation, this would send the invocation to the server
                setTimeout(() => {
                    console.log(`Method ${methodName} invoked successfully`);
                    resolve();
                }, 100);
            });
        }

        start() {
            console.log("Starting connection...");
            return new Promise((resolve, reject) => {
                setTimeout(() => {
                    this.connectionState = "Connected";
                    console.log("Connection started successfully");
                    resolve();
                }, 500);
            });
        }

        stop() {
            console.log("Stopping connection...");
            return new Promise((resolve, reject) => {
                setTimeout(() => {
                    this.connectionState = "Disconnected";
                    console.log("Connection stopped");
                    resolve();
                }, 500);
            });
        }
    }

    // LogLevel enum
    const LogLevel = {
        Trace: 0,
        Debug: 1,
        Information: 2,
        Warning: 3,
        Error: 4,
        Critical: 5,
        None: 6
    };

    // Export the API
    exports.HubConnectionBuilder = HubConnectionBuilder;
    exports.LogLevel = LogLevel;

    Object.defineProperty(exports, '__esModule', { value: true });
})));