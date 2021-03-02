var proConinatorApp = angular.module('proConinatorApp', ["xeditable"]);
proConinatorApp.run(function (editableOptions) {
    editableOptions.theme = 'bs3';
});

var proLightConst = 54;
var conLightConst = 58;

proConinatorApp.directive('weight', function () {
    return {
        restrict: 'E',
        scope: {
            min: '=',
            max: '=',
            btnClass: '=',
            value: '='
        },
        template: '<div class="input-group input-group-sm weightSelect">' +
          '<button type="button" class="input-group-addon btn btn-sm" ng-class="btnClass"  title="Remove Weight" ng-click="value = value - 1;" ng-disabled="value <= min"><span class="glyphicon glyphicon-minus"></span></button>' +
          '<input type="text" class="form-control" ng-model="value" value="1" title="weight"/>' +
          '<button type="button" class="input-group-addon btn btn-sm" ng-class="btnClass" title="Add Weight" ng-click="value = value + 1;" ng-disabled="value >= max"><span class="glyphicon glyphicon-plus"></span></button>' +
        '</div>'
    }
});

function topicItem(text, element) {
    var _this = this;
    // model properties
    _this.Title = text;
    _this.ProCons = [];
    _this.CreatedDate = new Date();
    _this.UpdatedDate = _this.CreatedDate;
}

var groups = [];

proConinatorApp.controller('ProConinatorController', ['$scope', '$window', '$http', function ($scope, $window, $http) {

    //called to initialize the scope options
    $scope.init = function (options) {
        $scope.proConGroups = typeof ($window.controllerInitOptions.proConGroups) !== "undefined" ? $window.controllerInitOptions.proConGroups : [];
        $scope.apiRootPath = typeof ($window.controllerInitOptions.apiRootPath) !== "undefined" ? $window.controllerInitOptions.apiRootPath : document.referrer;
    };

    //keeping this since groups only work for authenticated users. it makes that experience a little tricky to code. but not too painful.
    $scope.topics = [];

    $scope.loadGroups = function (userId) {
        $http({
            url: $scope.apiRootPath + "api/ProConStorageApi/GetList",
            type: "GET",
            params: { "userId": userId }

        }).success(function (data) {
            $scope.groups = data;
            if ($scope.groups.length > 0) {
                $scope.selectGroup($scope.groups[0]);
                initGroupPosition();
            }
        })
            .error(function () {
            });
    };

    $scope.createGroup = function (name, userId) {
        var newGroup =
            {
                "GroupName": name,
                "UserId": userId,
                "ProCons": []
            };
        if ($scope.groups != undefined && $scope.groups.length > 0) {
            $scope.groups.splice(0, 0, newGroup);
        }
        else {
            $scope.groups = [newGroup];
        }
        $scope.selectGroup(newGroup);
        $scope.saveGroup(true);

    };

    $scope.saveGroup = function (isNew) {
        if ($scope.selectedGroup != null) {
            $scope.selectedGroup.ModifiedDate = new Date();
            $scope.selectedGroup.ProConLists = $scope.topics;
            $http.post($scope.apiRootPath + "api/ProConStorageApi/SaveGroup",
                $scope.selectedGroup)
            .success(function (data) {
                if (isNew) {
                    $scope.selectedGroup.Id = data.Id;
                }
            });
        }
    };

    $scope.groupStatFunctions = [{
        "data": function (group) {//get highest pro
            var result = {};
            var lastHighest = 0;
            $.each(group.ProConLists, function (i) {
                var temp = $scope.topicSum(this, true);
                if (temp > lastHighest) {
                    lastHighest = temp;
                    var style = $scope.topicProColor(this);
                    style.display = "none";
                    result = {
                        "title": this.Title, "value": lastHighest, "style": style
                    };
                }
            });
            return result;
        }
    },
    {
        "data": function (group) {//get highest con
            var result = {};
            var lastHighest = 0;
            $.each(group.ProConLists, function (i) {
                var temp = $scope.topicSum(this, false);
                if (temp > lastHighest) {
                    lastHighest = temp;
                    var style = $scope.topicConColor(this);
                    style.display = "none";
                    result = { "title": this.Title, "value": lastHighest, "style": style };
                }
            });
            return result;
        }
    },
    {
        "data": function (group) {//get best pro ratio
            var result = {};
            var bestProRatio = 0.0;
            $.each(group.ProConLists, function (i) {
                var thisProSum = $scope.topicSum(this, true),
                    thisConSum = $scope.topicSum(this, false);
                var temp = thisProSum / (thisConSum || 1);

                if (temp > bestProRatio) {
                    bestProRatio = temp;
                    result = {
                        "title": this.Title, "value": thisProSum + ":" + thisConSum, "style": $scope.topicProColor(this)
                    };
                }
            });
            return result;
        }
    }];

    $scope.removeGroup = function (userId, groupId, index) {
        if (confirm("Are you sure you want to remove this group?")) {
            $scope.groups.splice(index, 1);
            if ($scope.groups.length > 0) {
                $scope.selectGroup($scope.groups[0]);
            } else {
                $scope.selectGroup({});
            }

            $http({
                url: $scope.apiRootPath + "api/ProConStorageApi/DeleteGroup",
                params: { "userId": userId, "groupId": groupId },
                method: "post"
            });
        }
    };

    $scope.selectedGroup = null;
    $scope.selectGroup = function (group) {
        $scope.topics = group.ProConLists != null ? group.ProConLists : [];
        $scope.selectedGroup = group;
    };
    $scope.isSelectedGroup = function (group) {
        return $scope.selectedGroup === group;
    };

    $scope.addTopic = function (text) {
        $scope.newProConWeight = 1;
        $scope.topics.splice(0, 0, new topicItem(text));
        $scope.saveGroup();
    };
    $scope.removeTopic = function (topics, index) {
        if (confirm("Are you sure you want to remove this topic?")) {
            topics.splice(index, 1);
            $scope.saveGroup();
        }
    };

    $scope.topicPros = function (topic) {
        var result = $(topic.ProCons).filter(function (i) {
            return this.ProCon == 0;
        }).toArray();
        return result;
    };
    $scope.topicCons = function (topic) {
        var result = $(topic.ProCons).filter(function (i) {
            return this.ProCon == 1;
        }).toArray();
        return result;
    };

    $scope.topicSum = function (topic, isPro) {
        var result = 0;

        $(isPro ? $scope.topicPros(topic) : $scope.topicCons(topic)).each(function () {
            result += this.Weight * 1;
        });
        return result;
    };

    $scope.topicProColor = function (topic) {
        var proSum = $scope.topicSum(topic, true);
        var maxLight = 20;
        var light = proSum >= 10 ?
            proLightConst : proLightConst + maxLight - (maxLight * (proSum * .1));
        var proHsl = "hsla(120, 39%, " + light + "%, 1)";
        return { "background-color": proHsl, "color": "#fff" };
    };
    $scope.topicConColor = function (topic) {
        var conSum = $scope.topicSum(topic, false);
        var maxLight = 20;
        var light = conSum >= 10 ?
            conLightConst : conLightConst + maxLight - (maxLight * (conSum * .1));
        var conHsl = "hsla(2, 64%, " + light + "%, 1)";
        return { "background-color": conHsl, "color": "#fff" };
    };

    $scope.addProCon = function (text, topic, proOrCon, weight) {
        var values = $(topic.ProCons).map(function(i){ return this.Id; });
        var id = (values.length > 0 ? Math.max.apply(null, values) : 0) + 1;

        var item = { "Id": id, "Text": text, "Weight": weight, "ProCon": proOrCon };
        topic.UpdatedDate = new Date();
        topic.ProCons.push(item);
        $scope.saveGroup($scope.selectedGroup);
    };

    $scope.removeProCon = function (topic, id) {
        if (confirm("Are you sure you want to remove this item?")) {
            topic.UpdatedDate = new Date();
            topic.ProCons = $(topic.ProCons).filter(function (i) {
                return this.Id != id;
            }).toArray();

            $scope.saveGroup();
        }
    };

    $scope.cancelEditProCon = function (item, form) {
        form.$cancel();
    };
}]);