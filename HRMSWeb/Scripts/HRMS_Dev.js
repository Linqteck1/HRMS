function tblToJson(rows) {
    var JsonArray = [];
    $(rows).each(function (i, e) {
        $(e).find("input[type=checkbox]").each(function (i, ee) {
            $(e).find("[name=" + ee.name + "][type=hidden]").val($(ee).prop("checked"));
        });
        $(e).find("input[type=radio]:checked").each(function (i, ee) {
            $(e).find("[name=" + ee.name + "][type=hidden]").val($(ee).val());
        });

        var JsonElem = {};
        $(e).find("input[type=hidden],input[type=text],input[type=Number],select,textarea").each(function (ii, ee) {
            JsonElem[ee.name] = $(ee).val();
        });
        JsonArray.push(JsonElem);
    });
    return JsonArray;
}

function disable_Controls(model) {
    $('#' + model + ' input,#' + model + ' select,#' + model + ' textarea').prop('disabled', true);
    $('#' + model + ' input[type=hidden]').prop('disabled', false);
    $('.bootstrap-select').attr('disabled', true);
    $('.bootstrap-select > button').css('cursor', 'not-allowed');


}