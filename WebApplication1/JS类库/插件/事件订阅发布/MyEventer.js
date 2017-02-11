//事件的订阅和发布插件
/*
var obj_margox = {
    'name': 'Margox',
    'age': 26,
    'sex': 'male',
    'isSingle': true,
    'girlFriend': null
}

Eventer(obj_margox);

*/

(function () {
    "use strict";//严模式


    window.Eventer= function Eventer(object) {

        if (!(this instanceof Eventer)) {//由于调用方法是Evnenter(obj), 所以第一次的this是window对象, if生效
            return new Eventer(object);//再次调用本身
        }

        if (typeof object !== 'object' || object instanceof Eventer) {
            throw new TypeError('Unable to bind object to Eventer.');
            return false;
        }

        delete object.__events__;//事件队列数据类型是{'eventName',[fun1, fun2......]}
        delete object.on;
        delete object.off;
        delete object.trigger;

        //在事件处理程序中存储当前操作对象.
        this.__object = object;
        //对象创建一个事件集合存储器
        object.__events__ = {};


        //对象增加订阅功能
        object.on = this.__on;
        //对象增加取消订阅功能
        object.off = this.__off;
        //对象触发事件
        object.trigger = this.__trigger;

    }

    /**
     *
     * 用来订阅事件，可单个或者多个
     * @param  {string | object} eventName 事件名，订阅单个事件时为一个字符串，订阅多个事件则需要传入一个包含事件名/函数的键值对
     * @param  {function} method 事件函数，仅订阅单个事件时才需要
     * @return {object} 返回自身以便于链式调用
     */
    Eventer.prototype.__on = function (eventName, method) { 
        var __event; 
        if (typeof eventName === "object") { //一次订阅多个事件
            for (__event in eventName) {
                if (Object.prototype.hasOwnProperty.call(eventName, __event) && (typeof eventName[__event] === "function")) {
                    this.on(__event, eventName[__event]);
                }
            } 
        } else if (typeof eventName === "string" && typeof method === "function") {
            //设置当前事件的响应存储数组
            this.__events__[eventName] || (this.__events__[eventName] = []);
            this.__events__[eventName].push(method); 
        } 
        return this; 
    }

    /**
     * 删除一个指定的事件队列
     * @param  {string} eventName 需要删除的事件名
     * @return {object} 返回自身以便于链式调用
     */
    Eventer.prototype.__off = function (eventName) {
        if (typeof eventName === 'string') {
            this.__events__[eventName] = [];
        }
        return this;
    }

    /**
     * 用于发布一个指定的事件
     * @param {string | ...any} 传入的第一个参数为需要发布的事件明，其后的参数会传递给事件队列中的每个函数
     * @return {object} 返回自身以便于链式调用
     */
    Eventer.prototype.__trigger = function () { 
        var __eventName = Array.prototype.shift.call(arguments),//shift,队列方法, 第一个成员出列. 数组长度-1
            __arguments = arguments,
            __returnFalse = false,//是否执行事件队列, 该功能用于执行中断事件队列
            __that = this,//保存this对象
            __return;//事件队列当前执行的操作函数的返回值

        if (typeof __eventName === 'string'
            && Object.prototype.hasOwnProperty.call(this.__events__, __eventName)//检查事件队列里面是否有正在触发的事件
            && (this.__events__[__eventName] instanceof Array)//检查正在触发的事件是否有处理队列
            ) {

            //使用ES5中Array的forEach枚举方法
            this.__events__[__eventName].forEach(function (method) {

                if (!__returnFalse) {//程序是否执行事件队列
                    __return = method.apply(__that, __arguments);//凡是遇到返回值为false的方法, 即为终止队列
                    __return === false && (__returnFalse = true);//设置终止队列
                }

            });

        } 
        return this; 
    }
})();


