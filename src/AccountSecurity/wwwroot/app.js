var app = angular.module('accountSecurityQuickstart', []);

app.controller('LoginController', function ($scope, $http, $window) {

    $scope.setup = {};
    
    $scope.login = function () {
        $http.post('/api/user/login', $scope.setup)
            .success(function (data, status, headers, config) {
                console.log("Login success: ", data);
                $window.location.href = $window.location.origin + "/2fa";
            })
            .error(function (data, status, headers, config) {
                console.error("Login error: ", data);
                alert("Error logging in.  Check console \n" + JSON.stringify(data));
            });
    };
});


app.controller('RegistrationController', function ($scope, $http, $window) {

    $scope.setup = {};

    $scope.register = function () {
        if ($scope.password === $scope.password2 && $scope.password !== "") {
            $scope.setup.password = $scope.password;

            $http.post('/api/user/register', $scope.setup)
                .success(function (data, status, headers, config) {
                    console.log("Success registering: ", data);
                    $window.location.href = $window.location.origin + "/2fa";
                })
                .error(function (data, status, headers, config) {
                    console.error("Registration error: ", data);
                    alert("Error registering.  Check console \n" + JSON.stringify(data));
                });
        } else {
            alert("Passwords do not match");
        }
    };
});

app.controller('AuthyController', function ($scope, $http, $window, $interval) {

    var pollingID;

    $scope.setup = {};

    $scope.logout = function () {
        $http.get('/api/user/logout')
            .success(function (data, status, headers, config) {
                console.log("Logout Response: ", data);
                $window.location.href = $window.location.origin + "/";
            })
            .error(function (data, status, headers, config) {
                console.error("Logout Error: ", data);
            });
    };

    /**
     * Request a token via SMS
     */
    $scope.sms = function () {
        $http.post('/api/accountsecurity/sms')
            .success(function (data, status, headers, config) {
                console.log("SMS sent: ", data);
            })
            .error(function (data, status, headers, config) {
                console.error("SMS error: ", data);
                alert("Problem sending SMS");
            });
    };

    /**
     * Request a Voice delivered token
     */
    $scope.voice = function () {
        $http.post('/api/accountsecurity/voice')
            .success(function (data, status, headers, config) {
                console.log("Phone call initialized: ", data);
            })
            .error(function (data, status, headers, config) {
                console.error("Voice call error: ", data);
                alert("Problem making Voice Call");
            });
    };

    /**
     * Verify a SMS, Voice or SoftToken
     */
    $scope.verify = function () {
        $http.post('/api/accountsecurity/verify', {token: $scope.setup.token})
            .success(function (data, status, headers, config) {
                console.log("2FA success ", data);
                $window.location.href = $window.location.origin + "/protected";
            })
            .error(function (data, status, headers, config) {
                console.error("Verify error: ", data);
                alert("Problem verifying token \n" + JSON.stringify(data));
            });
    };

    /**
     * Request a OneTouch transaction
     */
    $scope.onetouch = function () {
        $http.post('/api/accountsecurity/onetouch')
            .success(function (data, status, headers, config) {
                console.log("OneTouch success", data);
                /**
                 * Poll for the status change.  Every 5 seconds for 12 times.  1 minute.
                 */
                pollingID = $interval(oneTouchStatus, 5000, 12);
            })
            .error(function (data, status, headers, config) {
                console.error("Onetouch error: ", data);
                alert("Problem creating OneTouch request \n" + JSON.stringify(data));
            });
    };

    /**
     * Request the OneTouch status.
     */
    function oneTouchStatus() {
        $http.post('/api/accountsecurity/onetouchstatus')
            .success(function (data, status, headers, config) {
                console.log("OneTouch Status: ", data);
                if (data.approval_request.status === "approved") {
                    $window.location.href = $window.location.origin + "/protected";
                    $interval.cancel(pollingID);
                } else {
                    console.log("One Touch Request not yet approved");
                }
            })
            .error(function (data, status, headers, config) {
                console.log("OneTouch Polling Status: ", data);
                alert("Something went wrong with the OneTouch polling \n" + JSON.stringify(data));
                $interval.cancel(pollingID);
            });
    }
});

app.controller('PhoneVerificationController', function ($scope, $http, $window, $timeout) {

    $scope.setup = {
        via: "sms"
    };
    
    $scope.view = {
        start: true
    };

    /**
     * Initialize Phone Verification
     */
    $scope.startVerification = function () {
        $http.post('/api/verification/start', $scope.setup)
            .success(function (data, status, headers, config) {
                $scope.view.start = false;
                console.log("Verification started: ", data);
            })
            .error(function (data, status, headers, config) {
                console.error("Phone verification error: ", data);
                alert("Error verifying the token.  Check console for details. \n" + JSON.stringify(data));
            });
    };

    /**
     * Verify phone token
     */
    $scope.verifyToken = function () {
        $http.post('/api/verification/verify', $scope.setup)
            .success(function (data, status, headers, config) {
                console.log("Phone Verification Success success: ", data);
                $window.location.href = $window.location.origin + "/verified";
            })
            .error(function (data, status, headers, config) {
                console.error("Verification error: ", data);
                alert("Error verifying the token.  Check console for details.\n" + JSON.stringify(data));
            });
    };

    $scope.logout = function () {
            $window.location.href = $window.location.origin;
    };
});

