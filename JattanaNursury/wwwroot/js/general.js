let deletedId = null;

let _postDeleteUrl;
function initialize(postDeleteUrl)
{
    _postDeleteUrl = postDeleteUrl;
}

function deleteBtnClicked(btn) {
    const id = $(btn).data('id');
    deletedId = id;
    console.log(deletedId);
}

function deleteItem() {
    if (deletedId) {
        $.post(_postDeleteUrl, { id: deletedId }, function (res) {
            if (res.success) {
                window.location = res.url;
            }
        }).fail(function (res) {
            alert("Error while deleting, Contact IT");
        });
    }
}



