(function (app) {

    var EditController = function ($scope, ProductService) {

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
    EditController.$inject = ["$scope", "ProductService"];

    app.controller("EditController", EditController);

}(angular.module("AtTheProducts")));
