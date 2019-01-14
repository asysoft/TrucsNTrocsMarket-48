
    IH = 0;
    var ArrayImagesH = document.querySelectorAll("img[pubHaut]");
    ImaxH = ArrayImagesH.length - 1;

    IG = 0;
    var ArrayImagesG = document.querySelectorAll("img[pubgauche]");
    ImaxG = ArrayImagesG.length - 1;

    IDr = 0;
    var ArrayImagesD = document.querySelectorAll("img[pubdroite]");
    ImaxD = ArrayImagesD.length - 1;

    if (ImaxH > -1)
        setTimeout(suivanteH, 3000);

    if (ImaxG > -1)
        setTimeout(suivanteG, 3000);

    if (ImaxD > -1)
        setTimeout(suivanteD, 3000);

function suivanteH() {
    ArrayImagesH[IH].style.display = "none";
    if (IH < ImaxH)
        IH++;
    else
        IH = 0;
    ArrayImagesH[IH].style.display = "block";
    setTimeout(suivanteH, 3000);
}

function suivanteG() {
    ArrayImagesG[IG].style.display = "none";
    if (IG < ImaxG)
        IG++;
    else
        IG = 0;
    ArrayImagesG[IG].style.display = "block";
    setTimeout(suivanteG, 3000);
}

function suivanteD() {
    ArrayImagesD[IDr].style.display = "none";
    if (IDr < ImaxD)
        IDr++;
    else
        IDr = 0;
    ArrayImagesD[IDr].style.display = "block";
    setTimeout(suivanteD, 3000);
}

function ShowHidePub() {
    var pg = document.getElementById("divPubIndexGauche");
    var pd = document.getElementById("divPubIndexDroite");

    //if (pg.style.display == 'none' || pg.style.display == '') {
    //    pg.style.display = 'block';
    //} else if (pg.style.display == 'block') {
    //    pg.style.display = 'none';
    //}

    if (pg.style.display == '') {
        pg.style.display = 'none';
    } else if (pg.style.display == 'none') {
        pg.style.display = '';
    }
    if (pd.style.display == '') {
        pd.style.display = 'none';
    } else if (pd.style.display == 'none') {
        pd.style.display = '';
    }
}