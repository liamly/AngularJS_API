//(function (app) {
//    var run = function ($http) {
//        $http.defaults.headers.common.Accept = "application/json";
//    };

//    run.$inject = "$http";
//    app.run(run);
//})(angular.module("AtTheProducts", ["ngRoute", "ngMockE2E"]));

(function (app) {
    var config = function ($routeProvider, $httpProvider) {
        $routeProvider
            .when("/products", { templateUrl: "/Scripts/ngclient/views/list.html" })
            .when("/details/:id", { templateUrl: "/Scripts/ngclient/views/details.html" })
            .otherwise({ redirectTo: "/products" });
    };
    config.$inject = ["$routeProvider", "$httpProvider"];

    app.config(config);
    app.constant("ProductApiUrl", "http://localhost:52536/api/Products/");

}(angular.module("AtTheProducts", ["ngRoute", "ngResource", "ngAnimate"])));