function replacer(key, value) {
    if (value === null) {
        return undefined;
    }

    return value;
}

function Run(data, version) {
    var json = JSON.parse(data);

    // 만약 버전이 없는 파일인 경우
    if (json._Version === undefined) {
        if (Array.isArray(json)) {
            var temp = new Object();
            temp._Version = 0;
            temp.Data = json;
            json = temp;
        }
        else {
            json._Version = 0;
        }
    }

    json._Version = version;
    if (Array.isArray(json.Data)) {
        json.Data.forEach((item, index) => { migrate(item, index) });
    }
    else {
        migrate(json.Data, 0);
    }
    
    return JSON.stringify(json, replacer, 2);
}