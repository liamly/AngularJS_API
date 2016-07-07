(function(app) {

    var ProductsService = function ($http, ProductApiUrl) {

        var getAll = function() {
            return $http.get(ProductApiUrl);
        };

        var getById = function(id) {
            return $http.get(ProductApiUrl + id);
        };

        var update = function(Product) {
            return $http.put(ProductApiUrl + Product.ProductID, Product);
        };

        var create = function(Product) {
            return $http.post(ProductApiUrl, Product);
        };

        var destroy = function(Product) {
            return $http.delete(ProductApiUrl + Product.ProductID);
        };

        return {
            getAll: getAll,
            getById: getById,
            update: update,
            create: create,
            delete: destroy
        };
    };
   
    ProductsService.$inject = ["$http", "ProductApiUrl"];

    app.factory("ProductsService", ProductsService);

}(angular.module("ngProducts")))
