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
            //$scope.edit = { Product: { ProductName: "", QuantityPerUnit: "", UnitPrice: 0 } };
            $scope.edit.Product = angular.copy({ ProductName: "", QuantityPerUnit: "", UnitPrice: 0 });
        };
    
        $scope.delete = function (Product) {
            ProductService.delete(Product)
                .success(function() {
                    removeProductById(Product.ProductID);
                });
        };

        $scope.getProduct = function (Product) {
            $scope.Product = Product;
        };

        $scope.edit = function () {
            $scope.edit.Product = angular.copy($scope.Product);
        };

        var updateProduct = function () {
            ProductService.update($scope.edit.Product)
                .success(function () {
                    angular.extend($scope.Product, $scope.edit.Product);
                    $scope.edit.Product = null;
            });
        };

        var createProduct = function () {
            ProductService.create($scope.edit.Product)            
                .success(function (Product) {
                    $scope.Products.push(Product);
                    $scope.edit.Product = null;
                });
        };

        $scope.isEditable = function () {
            return $scope.edit && $scope.edit.Product;
        };

        $scope.cancel = function () {
            $scope.edit.Product = null;
        };

        $scope.save = function () {
            if ($scope.edit.Product.ProductID) {
                updateProduct();
            } else {
                createProduct();
            }
        };
    };
    ListController.$inject = ["$scope", "ProductService"];

    app.controller("ListController", ListController);

}(angular.module("AtTheProducts")));
