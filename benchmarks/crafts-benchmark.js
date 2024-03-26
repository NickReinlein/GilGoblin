import http from 'k6/http';
import {check, sleep} from 'k6';

export let options = {
    stages: [{duration: '5m', target: 50}, // Ramp-up to 50 VUs over 5 minutes
        {duration: '5m', target: 100}, // Ramp-up to 100 VUs over 5 minutes
        {duration: '10m', target: 100}, // Hold 100 VUs for 10 minutes
        {duration: '5m', target: 0}, // Ramp-down to 0 VUs over 5 minutes
    ], thresholds: {
        http_req_duration: ['p(98)<500'], // 98% of requests must complete below 500ms
    },
};

const worldIds = [21, 22, 23, 34];

export default function () {
    const delay = Math.floor(Math.random() * 500) + 1000;

    let randomIndex = Math.floor(Math.random() * worldIds.length);
    let worldId = worldIds[randomIndex];

    let response = http.get(`http://host.docker.internal:55448/craft/${worldId}`);

    check(response, {
        'status is 200': (r) => r.status === 200,
    });

    sleep(delay);
}