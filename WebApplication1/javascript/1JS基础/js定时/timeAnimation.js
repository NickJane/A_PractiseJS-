function getStyle(dom, attr) {
    return dom.currentStyle ? dom.currentStyle[attr] : getComputedStyle(dom)[attr];
}
function removeEvent(dom, event) {
    // if(document.body.de)
}

function move(dom, speed, distance, direction, callback) {
    var step = 0;//记录走了多远
    var control = window.setInterval(function () {
        var originalPosition = parseInt(getStyle(dom, direction), 10);
        //默认步长是20px一次
        originalPosition = distance > 0 ? (originalPosition + 20) : (originalPosition - 20);
        dom.style[direction] = originalPosition + 'px';
        step = distance > 0 ? (step + 20) : (step - 20);

        if (Math.abs(step) >= Math.abs(distance)) {
            clearInterval(control);
            callback && callback();
        }
        
    }, speed);
}
function shake(dom, direction, callback) {
    var arr = [];			// 20, -20, 18, -18 ..... 0

    var timer = null;

    for (var i = 20; i > 0; i -= 2) {
        arr.push(i, -i);
    }
    arr.push(0);

    clearInterval(timer);
    var num = 0;
    var pos = parseInt(getStyle(dom, direction));

    var tempobj = this;

    timer = window.setInterval(function () {
        dom.style[direction] = pos + arr[num] + 'px';
        num++;
        if (num == arr.length) {
            clearInterval(timer);


            //callback.call(dom);//dom 或者在上面把this另存为一个变量
            callback.call(tempobj);
        }
    }, 50);

}

function opacity(dom, speed, IsShow, callback) {
    //IsShow=1 =>从0->100
    //opacity: 0.3; filter: alpha( opacity:30);
    var step = IsShow ? 0 : 100;
    var control = null;
    control = window.setInterval(function () {
        step = IsShow ? step + 10 : step -10;
        if (dom.style['opacity'] != undefined)
            dom.style.opacity ="0."+ step;
        else
        {   
            step = IsShow && step > 80 ? 100 : step;
            step = !IsShow && step < 20 ? 0 : step;
            dom.style.filter = "alpha(opacity=" + step  + ")";
        }

        if (step <= 0 || step >= 100) {
            clearInterval(control);
            callback();
        }
    }, speed);

}