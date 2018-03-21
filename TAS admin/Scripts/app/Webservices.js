app.controller('GETMessenger', function ($scope, $http) {
	$http({
		method: "GET",
		url: "http://119.59.122.157/tms/Messenger"
	}).then(function mySuccess(response) {
		$scope.Messenger = response.data;
	}, function myError(response) {
		$scope.Messenger = response.statusText;
	});
})
app.controller('myCtrl11', function ($scope, $http) {
	$http({
		method: "POST",
		url: "http://119.59.122.157/tms/Messenger"
	}).then(function mySuccess(response) {
		$scope.Messenger = response.data;
	}, function myError(response) {
		$scope.Messenger = response.statusText;
	});
});