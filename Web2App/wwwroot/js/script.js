$(document).ready(function () {
    var defaultIcon = $("input[name='IconUri']").val();
    if (defaultIcon != null && defaultIcon != "" && defaultIcon != undefined) {
        $(".icon-view").addClass("active");
        $(".icon-view").attr("src", defaultIcon)
    }
    var constOperationId = "";
    var processCounter = 0;

    let protos = [];

    protos.push(new ProtoModel("1.0", "onepointzero"))
    protos.push(new ProtoModel("1.1", "onepointone"))

    function ProtoModel(key, value) {
        return { key: key, value: value }
    }

    $("input[name='IconUri']").change(function () {
        $(".icon-view").addClass("active");
        $(".icon-view").attr("src", $(this).val())

    })
    $(".submit-protocol").click(function () {



        let selectedProto = $("#proto_version").val();



        processCounter = 0;


        $(".list-group").empty();
        var spinner = $(` <div class="spinner-border" style="position:absolute;top:50%;left:50%;" role="status">
                        <span class="sr-only"></span>
                    </div>`);


        if ($(".modal").length != 0)
            $(".modal").remove();

        let protoName = "";

        for (let p of protos) {
            if (p.key == selectedProto) {
                protoName = p.value;
                break;
            }
        }
        console.log(protoName);
        var formData = new FormData();

        formData.append("OperationId", $("#operationId" + "_" + protoName).val());
        formData.append("Start", $("#NbfUTC" + "_" + protoName).val());
        formData.append("End", $("#ExpUTC" + "_" + protoName).val());
        formData.append("HostName", $("#HostName" + "_" + protoName).val());
        formData.append("Assignee", $("#Assignee" + "_" + protoName).val());
        formData.append("SecretKey", $("#secret" + "_" + protoName).val());
        formData.append("ClientId", $("#clientId" + "_" + protoName).val());
        formData.append("IconUri", $("#Icon" + "_" + protoName).val());
        formData.append("CallBackUrl", $("#Callback" + "_" + protoName).val());
        formData.append("Type", $("#type" + "_" + protoName).val());


        if ($("#dataURL" + "_" + protoName).val() == "" && selectedProto == "1.1") {
            let u = `https://scanme.sima.az/home/GetFileByOperationId/?operationId=${$("#operationId" + "_" + protoName).val()}&type=${$("#type" + "_" + protoName).val()}`;

            $("#dataURL" + "_" + protoName).val(u);

            formData.append("DataUrl", $("#dataURL" + "_" + protoName).val());

        }





        $.ajax({
            url: "/Home/QrCodeGenerate/?oldOperationId=" + constOperationId,
            data: formData,
            dataType: "json",
            contentType: false,
            processData: false,
            type: "POST",
            success: function (res) {
                console.log(res);
                if (res.status == 200) {
                    let mobileLink = res.url;
                    var qrElement = ` <div id="qrcode">
                                     <img style="width:350px;height:350px" src="/qrCodes/${res.fileName}" alt="qr code" />
                                     </div>
 <a class="btn btn-success mt-4" id="sima-opener" href="sima://web-to-app?data=${mobileLink}">Open Sima App</a>`;



                    $(".qr-wrapper").empty();
                    $(".qr-wrapper").append(qrElement);
                    constOperationId = res.operationId;
                    processCounter++;
                    $(".request-loaders").append(spinner);

                }
                else if (res.status == 400) {
                    alert("Please check data...");
                }
            },
            error: function (result) {
                console.log(result);
                alert("error")
            }
        });



        let getFileInterval = setInterval(function () {
            $.ajax({
                url: "/home/CheckSession/?processDesc=getfile&operationId=" + constOperationId,
                type: "get",
                success: function (response) {
                    if (response.status == 200) {
                        let li = document.createElement("li");
                        li.setAttribute("data-modal", response.data.guid)
                        li.style.cursor = "pointer";
                        li.classList.add("list-group-item");
                        li.innerText = `${processCounter}.1 ${response.data.desc}: ${response.data.operationId}`;
                        document.querySelector(".list-group").appendChild(li);

                        let modal = `<div style="overflow:auto;" data-modal="${response.data.guid}" class="modal fade" id="exampleModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
                            <div style="width:100%;overflow:auto;" class="modal-dialog" role="document">
                              <div style="width:100%;overflow:auto;" class="modal-content">
                                <div style="width:100%;overflow:auto;" class="modal-header">
                                  <h5 class="modal-title" id="exampleModalLabel">Information</h5>
                                </div>
                               <div style="width:100%;" class="modal-body">
                                                <h5>${response.data.desc}</h5>
                                                <p>${response.data.body}</p>
                                                <hr>
                                               <p>${response.data.headers}</p>
                                              </div>
                              </div>
                            </div>
                          </div>`;

                        $(".list-group").on("click", "li", function () {
                            $(`.modal[data-modal="${response.data.guid}"]`).modal("show");
                        })

                        $(".request-loaders .spinner-border").remove();
                        $(".list-group").css("display", "block");

                        $("body").append(modal);

                    }
                }
            });
        }, 1000);

        let callBackInterval = setInterval(function () {
            $.ajax({
                url: "/home/CheckSession/?processDesc=callback&operationId=" + constOperationId,
                type: "get",
                success: function (response) {
                    if (response.status == 200) {
                        let li = document.createElement("li");
                        li.setAttribute("data-modal", response.data.guid)
                        li.style.cursor = "pointer";
                        li.classList.add("list-group-item");
                        li.innerText = `${processCounter}.2 ${response.data.desc}: ${response.data.operationId}`;
                        document.querySelector(".list-group").appendChild(li);

                        let modal = `<div style="overflow:auto;" data-modal="${response.data.guid}" class="modal fade" id="exampleModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
                            <div style="width:100%;overflow:auto;" class="modal-dialog" role="document">
                              <div style="width:100%;overflow:auto;" class="modal-content">
                                <div style="width:100%;overflow:auto;" class="modal-header">
                                  <h5 class="modal-title" id="exampleModalLabel">Information</h5>
                                </div>
                               <div style="width:100%;" class="modal-body">
                                                <h5>${response.data.desc}</h5>
                                                <p>${response.data.body}</p>
                                                <hr>
                                               <p>${response.data.headers}</p>
                                              </div>
                              </div>
                            </div>
                          </div>`;

                        $(".list-group").on("click", "li", function () {
                            $(`.modal[data-modal="${response.data.guid}"]`).modal("show");
                        });

                        $("body").append(modal);

                        processCounter++;
                    }
                }
            });
        }, 1000)
    })



    $("#proto_version").change(function () {
        let protoVersion = $(this).val();
        $(".protocol").removeClass("active");
        $(`.protocol[data-proto='${protoVersion}']`).addClass("active");
    });

});