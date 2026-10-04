// modal.js - Handles AJAX Modals for Create, Edit, and Delete Confirmations

$(function () {
    // 1. Open Create or Edit Modal
    $(document).on('click', '[data-modal-url]', function (e) {
        e.preventDefault();
        const url = $(this).data('modal-url');
        const modalEl = document.getElementById('appModal');
        const modal = bootstrap.Modal.getOrCreateInstance(modalEl);

        $('#appModalContent').html(`
            <div class="modal-body text-center p-5">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
                <div class="mt-2 text-muted">Loading form...</div>
            </div>
        `);
        modal.show();

        $.get(url)
            .done(function (html) {
                $('#appModalContent').html(html);
                $.validator.unobtrusive.parse('#appModalContent form');
            })
            .fail(function (xhr) {
                const msg = xhr.responseJSON?.message || 'Failed to load form.';
                $('#appModalContent').html(`
                    <div class="modal-header">
                        <h5 class="modal-title text-danger">Error</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">${msg}</div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    </div>
                `);
            });
    });

    // 2. Submit Form inside Modal via AJAX
    $(document).on('submit', '#appModalContent form', function (e) {
        e.preventDefault();
        const $form = $(this);

        if (!$form.valid()) {
            return;
        }

        const submitBtn = $form.find('button[type="submit"]');
        const originalText = submitBtn.html();
        submitBtn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-1"></span> Saving...');

        $.ajax({
            url: $form.attr('action') || window.location.href,
            type: $form.attr('method') || 'POST',
            data: $form.serialize(),
            success: function (res) {
                if (res && res.success) {
                    bootstrap.Modal.getInstance(document.getElementById('appModal')).hide();
                    location.reload();
                } else if (typeof res === 'string') {
                    // Validation errors returned HTML partial
                    $('#appModalContent').html(res);
                    $.validator.unobtrusive.parse('#appModalContent form');
                } else if (res && !res.success) {
                    alert(res.message || 'Operation failed.');
                    submitBtn.prop('disabled', false).html(originalText);
                }
            },
            error: function (xhr) {
                submitBtn.prop('disabled', false).html(originalText);
                const msg = xhr.responseJSON?.message || 'An error occurred while saving.';
                alert(msg);
            }
        });
    });

    // 3. Delete Confirmation Dialog
    $(document).on('click', '[data-delete-url]', function (e) {
        e.preventDefault();
        const url = $(this).data('delete-url');
        const name = $(this).data('delete-name') || 'this item';
        const modalEl = document.getElementById('appModal');
        const modal = bootstrap.Modal.getOrCreateInstance(modalEl);

        const token = $('input[name="__RequestVerificationToken"]').first().val() || '';

        $('#appModalContent').html(`
            <div class="modal-header bg-danger text-white">
                <h5 class="modal-title"><i class="bi bi-exclamation-triangle-fill me-2"></i>Confirm Deletion</h5>
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body p-4">
                <p class="fs-5 mb-1">Are you sure you want to delete:</p>
                <div class="alert alert-secondary fw-bold text-break">${$('<div/>').text(name).html()}</div>
                <p class="text-muted small mb-0"><i class="bi bi-info-circle me-1"></i>This action cannot be undone.</p>
                <div id="deleteErrorContainer" class="alert alert-danger mt-3 d-none"></div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                <button type="button" class="btn btn-danger" id="btnConfirmDelete">
                    <i class="bi bi-trash"></i> Delete
                </button>
            </div>
        `);
        modal.show();

        $('#btnConfirmDelete').one('click', function () {
            const btn = $(this);
            btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-1"></span> Deleting...');

            $.post(url, { __RequestVerificationToken: token })
                .done(function (res) {
                    if (res && res.success) {
                        modal.hide();
                        location.reload();
                    } else {
                        btn.prop('disabled', false).html('<i class="bi bi-trash"></i> Delete');
                        $('#deleteErrorContainer').removeClass('d-none').text(res.message || 'Cannot delete item.');
                    }
                })
                .fail(function (xhr) {
                    btn.prop('disabled', false).html('<i class="bi bi-trash"></i> Delete');
                    const msg = xhr.responseJSON?.message || 'Delete operation failed.';
                    $('#deleteErrorContainer').removeClass('d-none').text(msg);
                });
        });
    });
});
