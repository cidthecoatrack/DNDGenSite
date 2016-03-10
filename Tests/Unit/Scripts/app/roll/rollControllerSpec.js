﻿'use strict'

describe('Roll Controller', function () {
    var vm;
    var rollServiceMock;
    var q;
    var scope;
    var sweetAlertServiceMock;

    beforeEach(module('app.roll'));

    beforeEach(function () {
        rollServiceMock = {
            getRoll: function (quantity, die) {
                if (quantity == 666)
                    return getMockedPromise({ "roll": quantity * die }, true);

                return getMockedPromise({ "roll": quantity });
            },
            getCustomRoll: function (quantity, die) {
                if (quantity == 666)
                    return getMockedPromise({ "roll": quantity * die }, true);

                return getMockedPromise({ "roll": quantity * die });
            },
            getExpressionRoll: function (expression) {
                if (expression == 'FAIL')
                    return getMockedPromise({ "roll": 42 * 600 }, true);

                return getMockedPromise({ "roll": 42 * 600 });
            },
            validateExpressionRoll: function (expression) {
                if (expression == 'FAIL')
                    return getMockedPromise({ "isValid": true }, true);

                if (expression == "invalid")
                    return getMockedPromise({ "isValid": false });

                return getMockedPromise({ "isValid": true });
            }
        };

        sweetAlertServiceMock = {};
        sweetAlertServiceMock.showError = jasmine.createSpy();
    });

    function getMockedPromise(body, shouldFail) {
        var deferred = q.defer();

        if (shouldFail)
            deferred.reject();
        else
            deferred.resolve(body);

        return deferred.promise;
    }

    beforeEach(inject(function ($rootScope, $controller, $q) {
        q = $q;
        scope = $rootScope.$new();

        vm = $controller('Roll as vm', {
            $scope: scope,
            rollService: rollServiceMock,
            sweetAlertService: sweetAlertServiceMock
        });
    }));

    it('has quantities of 1 at beginning', function () {
        expect(vm.standardQuantity).toBe(1);
        expect(vm.customQuantity).toBe(1);
    });

    it('has custom die of 1 at beginning', function () {
        expect(vm.customDie).toBe(1);
    });

    it('has empty expression at beginning', function () {
        expect(vm.expression).toBe('');
    });

    it('has roll of 0 at beginning', function () {
        expect(vm.roll).toBe(0);
    });

    it('is not rolling on load', function () {
        expect(vm.rolling).toBeFalsy();
    });

    it('is not validating on load', function () {
        expect(vm.validating).toBeFalsy();
    });

    it('has standard dice', function () {
        expect(vm.standardDice[0].name).toBe('2');
        expect(vm.standardDice[1].name).toBe('3');
        expect(vm.standardDice[2].name).toBe('4');
        expect(vm.standardDice[3].name).toBe('6');
        expect(vm.standardDice[4].name).toBe('8');
        expect(vm.standardDice[5].name).toBe('10');
        expect(vm.standardDice[6].name).toBe('12');
        expect(vm.standardDice[7].name).toBe('20');
        expect(vm.standardDice[8].name).toBe('Percentile');

        expect(vm.standardDice[0].die).toBe('d2');
        expect(vm.standardDice[1].die).toBe('d3');
        expect(vm.standardDice[2].die).toBe('d4');
        expect(vm.standardDice[3].die).toBe('d6');
        expect(vm.standardDice[4].die).toBe('d8');
        expect(vm.standardDice[5].die).toBe('d10');
        expect(vm.standardDice[6].die).toBe('d12');
        expect(vm.standardDice[7].die).toBe('d20');
        expect(vm.standardDice[8].die).toBe('d100');
    });

    it('has selected d20 as a standard dice on load', function () {
        expect(vm.standardDie).toEqual(vm.standardDice[7]);
    });

    it('rolls a standard die', function () {
        vm.standardQuantity = 9266;
        vm.standardDie = vm.standardDice[2];

        spyOn(rollServiceMock, 'getRoll').and.callThrough();

        vm.rollStandard();
        scope.$apply();

        expect(vm.roll).toBe(9266);
        expect(rollServiceMock.getRoll).toHaveBeenCalledWith(9266, 'd4');
    });

    it('rolls a custom roll', function () {
        vm.customQuantity = 9266;
        vm.customDie = 42;

        spyOn(rollServiceMock, 'getCustomRoll').and.callThrough();

        vm.rollCustom();
        scope.$apply();

        expect(vm.roll).toBe(9266 * 42);
        expect(rollServiceMock.getCustomRoll).toHaveBeenCalledWith(9266, 42);
    });

    it('rolls an expression', function () {
        vm.expression = 'expression';

        spyOn(rollServiceMock, 'getExpressionRoll').and.callThrough();

        vm.rollExpression();
        scope.$apply();

        expect(vm.roll).toBe(42 * 600);
        expect(rollServiceMock.getExpressionRoll).toHaveBeenCalledWith('expression');
    });

    it('says it is rolling while fetching a standard roll', function () {
        vm.standardQuantity = 9266;
        vm.standardDie = vm.standardDice[2];

        vm.rollStandard();

        expect(vm.rolling).toBeTruthy();
    });

    it('says it is done rolling while fetching a standard roll', function () {
        vm.standardQuantity = 9266;
        vm.standardDie = vm.standardDice[2];

        vm.rollStandard();
        scope.$apply();

        expect(vm.rolling).toBeFalsy();
    });

    it('says it is rolling while fetching a custom roll', function () {
        vm.customQuantity = 9266;
        vm.customDie = 42;

        vm.rollCustom();

        expect(vm.rolling).toBeTruthy();
    });

    it('says it is done rolling while fetching a custom roll', function () {
        vm.customQuantity = 9266;
        vm.customDie = 42;

        vm.rollCustom();
        scope.$apply();

        expect(vm.rolling).toBeFalsy();
    });

    it('says it is rolling while fetching an expression roll', function () {
        vm.expression = 'expression';
        vm.rollExpression();
        expect(vm.rolling).toBeTruthy();
    });

    it('says it is done rolling while fetching an expression roll', function () {
        vm.expression = 'expression';

        vm.rollExpression();
        scope.$apply();

        expect(vm.rolling).toBeFalsy();
    });

    it('shows an alert if an error is thrown when fetching a standard roll', function () {
        vm.standardQuantity = 666;
        vm.standardDie = vm.standardDice[2];

        vm.rollStandard();
        scope.$apply();

        expect(sweetAlertServiceMock.showError).toHaveBeenCalled();
    });

    it('is done rolling if an error is thrown when fetching a standard roll', function () {
        vm.standardQuantity = 666;
        vm.standardDie = vm.standardDice[2];

        vm.rollStandard();
        scope.$apply();

        expect(vm.rolling).toBeFalsy();
    });

    it('clears the roll if an error is thrown when fetching a standard roll', function () {
        vm.standardQuantity = 9266;
        vm.standardDie = vm.standardDice[2];

        vm.rollStandard();
        scope.$apply();

        vm.standardQuantity = 666;

        vm.rollStandard();
        scope.$apply();

        expect(vm.roll).toBe(0);
    });

    it('shows an alert if an error is thrown when fetching a custom roll', function () {
        vm.customQuantity = 666;
        vm.customDie = 42;

        vm.rollCustom();
        scope.$apply();

        expect(sweetAlertServiceMock.showError).toHaveBeenCalled();
    });

    it('is done rolling if an error is thrown when fetching a custom roll', function () {
        vm.customQuantity = 666;
        vm.customDie = 42;

        vm.rollCustom();
        scope.$apply();

        expect(vm.rolling).toBeFalsy();
    });

    it('clears the roll if an error is thrown when fetching a custom roll', function () {
        vm.customQuantity = 9266;
        vm.customDie = 42;

        vm.rollCustom();
        scope.$apply();

        vm.customQuantity = 666;

        vm.rollCustom();
        scope.$apply();

        expect(vm.roll).toBe(0);
    });

    it('shows an alert if an error is thrown when fetching an expression roll', function () {
        vm.expression = 'FAIL';

        vm.rollExpression();
        scope.$apply();

        expect(sweetAlertServiceMock.showError).toHaveBeenCalled();
    });

    it('is done rolling if an error is thrown when fetching an expression roll', function () {
        vm.expression = 'FAIL';

        vm.rollExpression();
        scope.$apply();

        expect(vm.rolling).toBeFalsy();
    });

    it('clears the roll if an error is thrown when fetching an expression roll', function () {
        vm.expression = 'expression';

        vm.rollExpression();
        scope.$apply();

        vm.expression = 'FAIL';

        vm.rollExpression();
        scope.$apply();

        expect(vm.roll).toBe(0);
    });

    it('validates a valid expression', function () {
        vm.expression = 'expression';
        scope.$digest();

        expect(vm.expressionIsValid).toBeTruthy();
    });

    it('validates an invalid expression', function () {
        vm.expression = 'invalid';
        scope.$digest();

        expect(vm.expressionIsValid).toBeFalsy();
    });

    it('shows an alert if an error is thrown when validating an expression roll', function () {
        vm.expression = 'FAIL';
        scope.$digest();

        expect(sweetAlertServiceMock.showError).toHaveBeenCalled();
    });

    it('says the expression is not valid if error is thrown', function () {
        vm.expression = 'expression';
        scope.$digest();

        vm.expression = 'FAIL';
        scope.$digest();

        expect(vm.expressionIsValid).toBeFalsy();
    });
})