var express = require('express');
var app = express();
var server = require('http').createServer(app);
var io = require('socket.io')(server);
var port = process.env.PORT || 3000;

app.use(express.static(__dirname + '/public'));

server.listen(port, function () {
    console.log('server listening at port %d', port)
});

io.on('connection', function (socket) {
    socket.on('message', function (data) {
        console.log('receive data: id = ' + data.id + ', value = ' + data.value);
        socket.broadcast.emit('message', data);
    });
});
