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
})
app.controller('GETGPS_DATA_devices', function ($scope, $http) {
	$http({
		method: "GET",
		url: "http://119.59.122.157/tms/GPS_DATA_devices"
	}).then(function mySuccess(response) {
		$scope.Messenger = response.data;
	}, function myError(response) {
		$scope.Messenger = response.statusText;
	});
})
app.controller('GETMessenger_join', function ($scope, $http) {
	$http({
		method: "GET",
		url: "http://119.59.122.157/tms/Messenger_join"
	}).then(function mySuccess(response) {
		$scope.Messenger = response.data;
	}, function myError(response) {
		$scope.Messenger = response.statusText;
	});
	$scope.action = 'open';
	$scope.noid = "";
	$scope.openmessenger = function (noid) {
		$scope.foo = true;
		$http.get("http://119.59.122.157/tms/Messenger/" + noid).then(function (response) {
			$scope.myWelcome = response.data;
		});
		//console.log($scope.foo);
		$scope.action = 'close';
	}
	$scope.closemessenger = function () {
		$scope.foo = false;
		//console.log($scope.foo);
		$scope.action = 'open';
	}
})

app.controller('GETHistory', function ($scope, $http) {
	//get history
	$scope.action = 'open';
	$scope.noid = "";
	$scope.openhistory = function () {
		$scope.his = true;
		// get history id
		//console.log($scope.his);
		$scope.action = 'close';
	}
	$scope.closehistory = function () {
		$scope.his = false;
		//console.log($scope.his);
		$scope.action = 'open';
	}
})

app.controller('TruckInfo', function ($scope, $http) {
	$scope.action = 'open';
	$scope.openData = function () {
		$scope.truckinsert = true;
		//
		//console.log($scope.truckinsert);
		$scope.action = 'close';
	}
	$scope.closeData = function () {
		$scope.truckinsert = false;
		//console.log($scope.truckinsert);
		$scope.action = 'open';
	}
	$scope.EditRowOftruckmas = function () {
		$scope.truckinsert = true;
		//
		//console.log($scope.truckinsert);
		$scope.action = 'close';
	}
	$scope.DelRowOftruckmas = function () {
		var txt;
		var r = confirm("คุณแน่ใจว่าจะลบรถคันนี้ !!");
		if (r == true) {
			txt = "You pressed OK!";
		} else {
			txt = "You pressed Cancel!";
		}
	}
})

app.controller('TEST', function ($scope, $http) {
	$scope.noid = {}
	$scope.TESTS = function To_Messenger_id(noid) {
		$scope.tid = noid;
		$http.get("http://119.59.122.157/tms/Messenger/?=" + $scope.tid).then(function (response) {
			$scope.myWelcome = response.data;
		});
	};
})

app.controller('demoController', function ($scope) {
	$scope.datas = [];       // เก็บข้อมูลทั้งหมดที่ถูกส่งจากฟอร์ม
	$scope.data = {};        // ข้อมูลที่ผูกกับ form
	$scope.action = 'add';
	// ฟังก์ชั่น เพิ่มข้อมูลลง datas
	$scope.addData = function () {
		// console.log('addData');
		$scope.datas.push($scope.data);
		$scope.data = {};
	};
	$scope.editvalue = function (data) {
		// console.log('EditValue');
		$scope.action = 'edit';
		// $scope.data = data;
		$scope.data.shipmentno = data.shipmentno;
		$scope.data.driver = data.driver;
		$scope.data.truckno = data.truckno;
		$scope.data.deliveryno = data.deliveryno;
		$scope.data.orderno = data.orderno;
		$scope.data.duedate = data.duedate;
		$scope.data.loading = data.loading;
		$scope.data.destination = data.destination;
	}
	$scope.savevalue = function () {
		// console.log($scope.datas[0]);
		for (var i = 0; i < $scope.datas.length; i++) {
			if ($scope.datas[i].orderno === $scope.data.orderno) {
				$scope.datas[i].shipmentno = $scope.data.shipmentno;
				$scope.datas[i].driver = $scope.data.driver;
				$scope.datas[i].truckno = $scope.data.truckno;
				$scope.datas[i].deliveryno = $scope.data.deliveryno;
				$scope.datas[i].orderno = $scope.data.orderno;
				$scope.datas[i].duedate = $scope.data.duedate;
				$scope.datas[i].loading = $scope.data.loading;
				$scope.datas[i].destination = $scope.data.destination;
				break;
			}
		}
		
		$scope.action = 'add';
		$scope.data = {};
	}
	$scope.deletevalue = function (data) {
		var txt;
		var r = confirm("คุณแน่ใจว่าจะลบรายชื่อนี้ !!");
		if (r == true) {
			txt = "You pressed OK!";
			for (var i = 0; i < $scope.datas.length; i++) {
				if ($scope.datas[i].orderno === data.orderno) {
					$scope.datas.splice(i, 1);
					break;
				}
			}
		} else {
			txt = "You pressed Cancel!";
		}
		
	}

});

