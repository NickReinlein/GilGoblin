import http from 'k6/http';
import {check, sleep} from 'k6';

export let options = {
    stages: [
        {duration: '1m', target: 5},
        {duration: '3m', target: 5},
        {duration: '1m', target: 10},
        {duration: '3m', target: 10},
        {duration: '1m', target: 5},
        {duration: '3m', target: 5},
        {duration: '1m', target: 0},
    ],
};

const worldIds = [21, 22, 23, 34];

export default function (delay = 50) {
    let randomIndex = Math.floor(Math.random() * worldIds.length);
    let worldId = worldIds[randomIndex];

    let response = http.get(`http://localhost:55448/craft/${worldId}`);

    check(response, {
        'status is 200': (r) => r.status === 200,
    });

    sleep(delay);
}
