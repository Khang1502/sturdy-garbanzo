$(function () {
    $(".catalog-select").each(function () {
        const select = $(this), info = $("#" + this.id + "-info");
        let pending;
        function load() {
            if (pending) pending.abort();
            info.empty();
            if (!select.val()) return;
            pending = $.getJSON(select.data("info-url"), { id: select.val() })
                .done(function (data) { info.text(data.ten + (data.diachi ? " — " + data.diachi : "")); })
                .fail(function (_, status) { if (status !== "abort") info.text("Không tải được thông tin danh mục."); });
        }
        select.on("change", load); load();
    });
    let previewUrl;
    $("#anh").on("change", function () {
        if (previewUrl) URL.revokeObjectURL(previewUrl);
        const f = this.files[0];
        if (!f) return;
        if (f.size > 2 * 1024 * 1024 || !["image/jpeg", "image/png", "image/gif", "image/webp"].includes(f.type)) {
            this.value = ""; alert("Chọn ảnh JPG, PNG, GIF hoặc WebP không quá 2 MB."); return;
        }
        previewUrl = URL.createObjectURL(f); $("#image-preview").attr("src", previewUrl);
    });
});
