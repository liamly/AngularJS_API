(function (app) {

    var ListController = function ($scope, ProductService) {

        var removeProductById = function(id) {
            for (var i = 0; i < $scope.Products.length; i++) {
                if ($scope.Products[i].ProductID == id) {
                    $scope.Products.splice(i, 1);
                }
            }
        };

        ProductService.getAll().success(function(Products) {
            $scope.Products = Products;
        });

        $scope.create = function () {
            $scope.edit = { Product: { ProductName: "", QuantityPerUnit: "", UnitPrice: 0 } };
        };
    
        $scope.delete = function (Product) {
            ProductService.delete(Product)
                .success(function() {
                    removeProductById(Product.ProductID);
                });
        };
    };
    ListController.$inject = ["$scope", "ProductService"];

    app.controller("ListController", ListController);

}(angular.module("AtTheProducts")));
