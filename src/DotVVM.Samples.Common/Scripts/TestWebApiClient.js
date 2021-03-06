var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var TestWebApiClient;
(function (TestWebApiClient) {
    /* tslint:disable */
    //----------------------
    // <auto-generated>
    //     Generated using the NSwag toolchain v11.3.5.0 (NJsonSchema v9.4.5.0) (http://NSwag.org)
    // </auto-generated>
    //----------------------
    // ReSharper disable InconsistentNaming
    var CompaniesClient = (function () {
        function CompaniesClient(baseUrl, http) {
            this.jsonParseReviver = undefined;
            this.http = http ? http : window;
            this.baseUrl = baseUrl ? baseUrl : "";
        }
        CompaniesClient.prototype.get = function () {
            var _this = this;
            var url_ = this.baseUrl + "/api/Companies";
            url_ = url_.replace(/[?&]$/, "");
            var options_ = {
                method: "GET",
                headers: {
                    "Content-Type": "application/json",
                    "Accept": "application/json"
                }
            };
            return this.http.fetch(url_, options_).then(function (_response) {
                return _this.processGet(_response);
            });
        };
        CompaniesClient.prototype.processGet = function (response) {
            var _this = this;
            var status = response.status;
            if (status === 200) {
                return response.text().then(function (_responseText) {
                    var result200 = null;
                    var resultData200 = _responseText === "" ? null : JSON.parse(_responseText, _this.jsonParseReviver);
                    if (resultData200 && resultData200.constructor === Array) {
                        result200 = [];
                        for (var _i = 0, resultData200_1 = resultData200; _i < resultData200_1.length; _i++) {
                            var item = resultData200_1[_i];
                            result200.push(Company.fromJS(item));
                        }
                    }
                    return result200;
                });
            }
            else if (status !== 200 && status !== 204) {
                return response.text().then(function (_responseText) {
                    return throwException("An unexpected server error occurred.", status, _responseText);
                });
            }
            return Promise.resolve(null);
        };
        return CompaniesClient;
    }());
    TestWebApiClient.CompaniesClient = CompaniesClient;
    var OrdersClient = (function () {
        function OrdersClient(baseUrl, http) {
            this.jsonParseReviver = undefined;
            this.http = http ? http : window;
            this.baseUrl = baseUrl ? baseUrl : "";
        }
        OrdersClient.prototype.get = function (companyId, pageIndex, pageSize) {
            var _this = this;
            var url_ = this.baseUrl + "/api/Orders/{companyId}?";
            if (companyId === undefined || companyId === null)
                throw new Error("The parameter 'companyId' must be defined.");
            url_ = url_.replace("{companyId}", encodeURIComponent("" + companyId));
            if (pageIndex === null)
                throw new Error("The parameter 'pageIndex' cannot be null.");
            else if (pageIndex !== undefined)
                url_ += "pageIndex=" + encodeURIComponent("" + pageIndex) + "&";
            if (pageSize === null)
                throw new Error("The parameter 'pageSize' cannot be null.");
            else if (pageSize !== undefined)
                url_ += "pageSize=" + encodeURIComponent("" + pageSize) + "&";
            url_ = url_.replace(/[?&]$/, "");
            var options_ = {
                method: "GET",
                headers: {
                    "Content-Type": "application/json",
                    "Accept": "application/json"
                }
            };
            return this.http.fetch(url_, options_).then(function (_response) {
                return _this.processGet(_response);
            });
        };
        OrdersClient.prototype.processGet = function (response) {
            var _this = this;
            var status = response.status;
            if (status === 200) {
                return response.text().then(function (_responseText) {
                    var result200 = null;
                    var resultData200 = _responseText === "" ? null : JSON.parse(_responseText, _this.jsonParseReviver);
                    if (resultData200 && resultData200.constructor === Array) {
                        result200 = [];
                        for (var _i = 0, resultData200_2 = resultData200; _i < resultData200_2.length; _i++) {
                            var item = resultData200_2[_i];
                            result200.push(Order.fromJS(item));
                        }
                    }
                    return result200;
                });
            }
            else if (status !== 200 && status !== 204) {
                return response.text().then(function (_responseText) {
                    return throwException("An unexpected server error occurred.", status, _responseText);
                });
            }
            return Promise.resolve(null);
        };
        OrdersClient.prototype.getItem = function (orderId) {
            var _this = this;
            var url_ = this.baseUrl + "/api/Orders/{orderId}";
            if (orderId === undefined || orderId === null)
                throw new Error("The parameter 'orderId' must be defined.");
            url_ = url_.replace("{orderId}", encodeURIComponent("" + orderId));
            url_ = url_.replace(/[?&]$/, "");
            var options_ = {
                method: "GET",
                headers: {
                    "Content-Type": "application/json",
                    "Accept": "application/json"
                }
            };
            return this.http.fetch(url_, options_).then(function (_response) {
                return _this.processGetItem(_response);
            });
        };
        OrdersClient.prototype.processGetItem = function (response) {
            var _this = this;
            var status = response.status;
            if (status === 200) {
                return response.text().then(function (_responseText) {
                    var result200 = null;
                    var resultData200 = _responseText === "" ? null : JSON.parse(_responseText, _this.jsonParseReviver);
                    result200 = resultData200 ? Order.fromJS(resultData200) : null;
                    return result200;
                });
            }
            else if (status !== 200 && status !== 204) {
                return response.text().then(function (_responseText) {
                    return throwException("An unexpected server error occurred.", status, _responseText);
                });
            }
            return Promise.resolve(null);
        };
        OrdersClient.prototype.put = function (orderId, order) {
            var _this = this;
            var url_ = this.baseUrl + "/api/Orders/{orderId}";
            if (orderId === undefined || orderId === null)
                throw new Error("The parameter 'orderId' must be defined.");
            url_ = url_.replace("{orderId}", encodeURIComponent("" + orderId));
            url_ = url_.replace(/[?&]$/, "");
            var content_ = JSON.stringify(order);
            var options_ = {
                body: content_,
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "Accept": "application/json"
                }
            };
            return this.http.fetch(url_, options_).then(function (_response) {
                return _this.processPut(_response);
            });
        };
        OrdersClient.prototype.processPut = function (response) {
            var status = response.status;
            if (status === 200 || status === 206) {
                var contentDisposition = response.headers ? response.headers.get("content-disposition") : undefined;
                var fileNameMatch = contentDisposition ? /filename="?([^"]*)"?;/g.exec(contentDisposition) : undefined;
                var fileName_1 = fileNameMatch && fileNameMatch.length > 1 ? fileNameMatch[1] : undefined;
                var headers_1 = {};
                if (response.headers && response.headers.forEach) {
                    response.headers.forEach(function (v, k) { return headers_1[k] = v; });
                }
                ;
                return response.blob().then(function (blob) { return { fileName: fileName_1, data: blob, status: status, headers: headers_1 }; });
            }
            else if (status !== 200 && status !== 204) {
                return response.text().then(function (_responseText) {
                    return throwException("An unexpected server error occurred.", status, _responseText);
                });
            }
            return Promise.resolve(null);
        };
        OrdersClient.prototype.post = function (order) {
            var _this = this;
            var url_ = this.baseUrl + "/api/Orders";
            url_ = url_.replace(/[?&]$/, "");
            var content_ = JSON.stringify(order);
            var options_ = {
                body: content_,
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Accept": "application/json"
                }
            };
            return this.http.fetch(url_, options_).then(function (_response) {
                return _this.processPost(_response);
            });
        };
        OrdersClient.prototype.processPost = function (response) {
            var status = response.status;
            if (status === 200 || status === 206) {
                var contentDisposition = response.headers ? response.headers.get("content-disposition") : undefined;
                var fileNameMatch = contentDisposition ? /filename="?([^"]*)"?;/g.exec(contentDisposition) : undefined;
                var fileName_2 = fileNameMatch && fileNameMatch.length > 1 ? fileNameMatch[1] : undefined;
                var headers_2 = {};
                if (response.headers && response.headers.forEach) {
                    response.headers.forEach(function (v, k) { return headers_2[k] = v; });
                }
                ;
                return response.blob().then(function (blob) { return { fileName: fileName_2, data: blob, status: status, headers: headers_2 }; });
            }
            else if (status !== 200 && status !== 204) {
                return response.text().then(function (_responseText) {
                    return throwException("An unexpected server error occurred.", status, _responseText);
                });
            }
            return Promise.resolve(null);
        };
        OrdersClient.prototype["delete"] = function (orderId) {
            var _this = this;
            var url_ = this.baseUrl + "/api/Orders/delete-{orderId}";
            if (orderId === undefined || orderId === null)
                throw new Error("The parameter 'orderId' must be defined.");
            url_ = url_.replace("{orderId}", encodeURIComponent("" + orderId));
            url_ = url_.replace(/[?&]$/, "");
            var options_ = {
                method: "DELETE",
                headers: {
                    "Content-Type": "application/json"
                }
            };
            return this.http.fetch(url_, options_).then(function (_response) {
                return _this.processDelete(_response);
            });
        };
        OrdersClient.prototype.processDelete = function (response) {
            var status = response.status;
            if (status === 204) {
                return response.text().then(function (_responseText) {
                    return;
                });
            }
            else if (status !== 200 && status !== 204) {
                return response.text().then(function (_responseText) {
                    return throwException("An unexpected server error occurred.", status, _responseText);
                });
            }
            return Promise.resolve(null);
        };
        return OrdersClient;
    }());
    TestWebApiClient.OrdersClient = OrdersClient;
    var Company = (function () {
        function Company(data) {
            if (data) {
                for (var property in data) {
                    if (data.hasOwnProperty(property))
                        this[property] = data[property];
                }
            }
        }
        Company.prototype.init = function (data) {
            if (data) {
                this.id = data["Id"] !== undefined ? data["Id"] : null;
                this.name = data["Name"] !== undefined ? data["Name"] : null;
                this.owner = data["Owner"] !== undefined ? data["Owner"] : null;
            }
        };
        Company.fromJS = function (data) {
            var result = new Company();
            result.init(data);
            return result;
        };
        Company.prototype.toJSON = function (data) {
            data = typeof data === 'object' ? data : {};
            data["Id"] = this.id !== undefined ? this.id : null;
            data["Name"] = this.name !== undefined ? this.name : null;
            data["Owner"] = this.owner !== undefined ? this.owner : null;
            return data;
        };
        return Company;
    }());
    TestWebApiClient.Company = Company;
    var Order = (function () {
        function Order(data) {
            if (data) {
                for (var property in data) {
                    if (data.hasOwnProperty(property))
                        this[property] = data[property];
                }
            }
        }
        Order.prototype.init = function (data) {
            if (data) {
                this.id = data["Id"] !== undefined ? data["Id"] : null;
                this.number = data["Number"] !== undefined ? data["Number"] : null;
                this.date = data["Date"] ? new Date(data["Date"].toString()) : null;
                this.companyId = data["CompanyId"] !== undefined ? data["CompanyId"] : null;
                if (data["OrderItems"] && data["OrderItems"].constructor === Array) {
                    this.orderItems = [];
                    for (var _i = 0, _a = data["OrderItems"]; _i < _a.length; _i++) {
                        var item = _a[_i];
                        this.orderItems.push(OrderItem.fromJS(item));
                    }
                }
            }
        };
        Order.fromJS = function (data) {
            var result = new Order();
            result.init(data);
            return result;
        };
        Order.prototype.toJSON = function (data) {
            data = typeof data === 'object' ? data : {};
            data["Id"] = this.id !== undefined ? this.id : null;
            data["Number"] = this.number !== undefined ? this.number : null;
            data["Date"] = this.date ? this.date.toISOString() : null;
            data["CompanyId"] = this.companyId !== undefined ? this.companyId : null;
            if (this.orderItems && this.orderItems.constructor === Array) {
                data["OrderItems"] = [];
                for (var _i = 0, _a = this.orderItems; _i < _a.length; _i++) {
                    var item = _a[_i];
                    data["OrderItems"].push(item.toJSON());
                }
            }
            return data;
        };
        return Order;
    }());
    TestWebApiClient.Order = Order;
    var OrderItem = (function () {
        function OrderItem(data) {
            if (data) {
                for (var property in data) {
                    if (data.hasOwnProperty(property))
                        this[property] = data[property];
                }
            }
        }
        OrderItem.prototype.init = function (data) {
            if (data) {
                this.id = data["Id"] !== undefined ? data["Id"] : null;
                this.text = data["Text"] !== undefined ? data["Text"] : null;
                this.amount = data["Amount"] !== undefined ? data["Amount"] : null;
                this.discount = data["Discount"] !== undefined ? data["Discount"] : null;
                this.isOnStock = data["IsOnStock"] !== undefined ? data["IsOnStock"] : null;
            }
        };
        OrderItem.fromJS = function (data) {
            var result = new OrderItem();
            result.init(data);
            return result;
        };
        OrderItem.prototype.toJSON = function (data) {
            data = typeof data === 'object' ? data : {};
            data["Id"] = this.id !== undefined ? this.id : null;
            data["Text"] = this.text !== undefined ? this.text : null;
            data["Amount"] = this.amount !== undefined ? this.amount : null;
            data["Discount"] = this.discount !== undefined ? this.discount : null;
            data["IsOnStock"] = this.isOnStock !== undefined ? this.isOnStock : null;
            return data;
        };
        return OrderItem;
    }());
    TestWebApiClient.OrderItem = OrderItem;
    var SwaggerException = (function (_super) {
        __extends(SwaggerException, _super);
        function SwaggerException(message, status, response, result) {
            var _this = _super.call(this) || this;
            _this.message = message;
            _this.status = status;
            _this.response = response;
            _this.result = result;
            return _this;
        }
        return SwaggerException;
    }(Error));
    TestWebApiClient.SwaggerException = SwaggerException;
    function throwException(message, status, response, result) {
        if (result !== null && result !== undefined)
            throw result;
        else
            throw new SwaggerException(message, status, response, null);
    }
})(TestWebApiClient || (TestWebApiClient = {}));
