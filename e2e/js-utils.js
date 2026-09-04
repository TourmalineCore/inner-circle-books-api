function fn() {
    function decodeString(string) {
        var Bytes = Java.type('java.util.Base64');
        var decodedBytes = Bytes.getDecoder().decode(string);
        
        return new java.lang.String(decodedBytes);
    }

    return {
        getAuthHeaders: function (tokenValue) {
            return {
                [this.getAuthHeaderKey()]: this.getAuthHeaderValue(tokenValue)
            }
        },

        getAuthHeaderKey: function () {
            return this.shouldUseFakeExternalDependencies()
                ? 'X-DEBUG-TOKEN'
                : 'Authorization';
        },

        getAuthHeaderValue: function (tokenValue) {
            // the debug token is the payload part of the jwt on its own,
            // without the header and the signature
            return this.shouldUseFakeExternalDependencies()
                ? tokenValue.split('.')[1]
                : 'Bearer ' + tokenValue;
        },

        shouldUseFakeExternalDependencies: function () {
            return this.getEnvVariable('SHOULD_USE_FAKE_EXTERNAL_DEPENDENCIES') === 'true';            
        },

        getEnvVariable: function (variable) {
            var System = Java.type('java.lang.System');

            return System.getenv(variable);
        },

        getDateTwoMonthsLaterThanCurrent: function () {
            const date = new Date();
            date.setMonth(date.getMonth() + 3);

            return date.toISOString().split('T')[0];
        },

        getEmployeeIdFromToken: function (tokenValue) {
            var decodedString = decodeString(tokenValue.split('.')[1]);
            var tokenData = JSON.parse(decodedString);

            return tokenData.employeeId;
        }
    }
}