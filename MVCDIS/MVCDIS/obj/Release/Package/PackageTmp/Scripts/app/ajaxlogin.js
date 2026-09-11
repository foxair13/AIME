$(function () {
    var getValidationSummaryErrors = function ($form) {
        var errorSummary = $form.find('.validation-summary-errors, .validation-summary-valid');
        return errorSummary;
    };

    var displayErrors = function (form, errors) {
        var errorSummary = getValidationSummaryErrors(form)
            .removeClass('validation-summary-valid')
            .addClass('validation-summary-errors');

        var items = $.map(errors, function (error) {
            return '<li>' + error + '</li>';
        }).join('');

        errorSummary.find('ul').empty().append(items);
    };

    var formSubmitHandler = function (e) {
        var $form = $(this);

        // We check if jQuery.validator exists on the form
        if (!$form.valid || $form.valid()) {
            $.post($form.attr('action'), $form.serializeArray())
                .done(function (json) {
                    json = json || {};

                    // In case of success, we redirect to the provided URL or the same page.

                    if (json.success) {
                        if (json.redirect == 0) {
                            $("#divResult").text(json.datatext.toString());
                            $("#HideRegister").slideUp(300);
                        } else
                            if (json.redirect == 1) {
                                $("#HideRecover").slideUp(300);
                                $("#divReсover").text(json.datatext.toString());
                            }
                            else 
                                if (json.redirect == 2)
                                {
                                    $("#HideReset").slideUp(300);
                                    $("#divReset").text(json.datatext.toString());
                                    
                                }
                                else
                                {
                                window.location = json.redirect || 'http://' + location.host.toString();
                            }
                    } else if (json.errors) {
                        displayErrors($form, json.errors);
                    }
                })
                .error(function () {
                    displayErrors($form, ['Возникла системная ошибка.']);
                });
        }

        // Prevent the normal behavior since we opened the dialog
        e.preventDefault();
    };

    $("#loginForm").submit(formSubmitHandler);
    $("#recoverForm").submit(formSubmitHandler);
    $("#registerForm").submit(formSubmitHandler);
    $("#resetForm").submit(formSubmitHandler);

});