var baseURL = window.location.protocol +
    "//" + window.location.hostname +
    (window.location.port ? ':'
        + window.location.port : '');
$.ajax({
    type: 'GET',
    url: baseURL +
        "/api/BatalhasAPI/QtdBatalhas"
})
    .done(
        function (data) {
            document
                .getElementById("qtdBatalhas")
                .innerHTML = data;
        }
    )
    .fail()

$.ajax({
    type: 'GET',
    url: baseURL +
        "/api/BatalhasAPI/QtdBatalhasJogador"
})
    .done(
        function (data) {
            document
                .getElementById("qtdBatalhasJogador")
                .innerHTML = data;
        }
    )
    .fail();
function criarJogo() {

    var rad = document.selectExercito.exercito;

    for (var i = 0; i < rad.length; i++) {
        if (rad[i].checked) {
            var idNacao = rad[i].value;
        }
    }

    $.ajax({
        type: 'GET',
        url: baseURL +
            "/api/BatalhasAPI/CriarBatalha/" + idNacao 
    })
        .done(
            function (data) {
                window.location.href = "/Batalhas/Lobby/" + data.id;
            }
        )
        .fail(
            function () {
                alert("Erro ao Criar a Batalha.")
            });
}