angular.module('ClientAngularApp', [])

    .controller('MoviesController', function ($scope, $http)
    {
        // this will do for now
        //$scope.appTitle = "Movies";
        //$scope.movies =
        //[
        //    { Id: 1, Name: "Fight Club",Director: "David Fincher" },
        //    { Id: 2, Name: "Into The Wild", Director: "Sean Penn" },
        //    { Id: 3, Name: "Dancer in the Dark", Director:"Lars von Trier " }
        //];

        $http({
            method: 'GET',
            url: 'http://localhost:52536/api/Order'
        })
         .success(function (data)
         {
             $scope.appTitle = "Movies";
             $scope.movies = data;
         })
        .error(function (data) {
            alert(data);
        });
        

    });