var myAutocompleteHelper = /** @class */ (function () {
    function myAutocompleteHelper() {
    }
    myAutocompleteHelper.prototype.configAutoCompleteTags = function (startSearchCallback, selectResultCallback, autoCompleteControlName) {
        if (autoCompleteControlName === void 0) { autoCompleteControlName = '#searchImageTag'; }
        $(autoCompleteControlName).keyup(function (event) {
            var token = $("#keyForm input[name=__RequestVerificationToken]").val();
            var searchText = $(autoCompleteControlName).val().toString();
            if (searchText.length < 2) {
                return;
            }
            var formData = new FormData();
            formData.append("searchData", searchText);
            if (startSearchCallback) {
                startSearchCallback(searchText);
            }
            $.ajax({
                url: '/tags/AutoCompleteSearch',
                data: formData,
                processData: false,
                contentType: false,
                type: 'POST',
                headers: {
                    'X-CSRF-TOKEN': token.toString(),
                },
                success: function (data) {
                    var availableData = [];
                    data.forEach(function (element) {
                        availableData.push({ id: element.id, label: element.name });
                    });
                    $(autoCompleteControlName).autocomplete({
                        source: availableData,
                        select: function (event, ui) {
                            $(autoCompleteControlName).val(ui.item.value);
                            if (selectResultCallback) {
                                selectResultCallback(ui.item);
                            }
                            event.returnValue = false;
                            return false;
                        }
                    });
                },
                error: function (data) {
                    alert(data);
                }
            });
        });
    };
    return myAutocompleteHelper;
}());
//# sourceMappingURL=myAutoComplete.js.map