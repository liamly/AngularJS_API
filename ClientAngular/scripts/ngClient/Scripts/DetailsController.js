(function (app) {

    var DetailsController = function ($scope, $routeParams, ProductService) {

        ProductService.getById($routeParams.id)
                    .success(function(Product) {
                        $scope.Product = Product;
                    });

        $scope.edit = function () {
            $scope.edit.Product = angular.copy($scope.getProduct.Product);
        };
    };
    DetailsController.$inject = ["$scope", "$routeParams", "ProductService"];

    app.controller("DetailsController", DetailsController);

}(angular.module("AtTheProducts")));
