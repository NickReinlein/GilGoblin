import http from 'k6/http';
import {check, sleep} from 'k6';

export let options = {
    stages: [
        {duration: '20s', target: 2},
        {duration: '30s', target: 5},
        // {duration: '1m', target: 10},
        // {duration: '1m', target: 5},
        // {duration: '1m', target: 2},
    ],
};

const worldIds = [21, 22, 23, 34];

export default function (delay = 5) {
    let randomIndex = Math.floor(Math.random() * worldIds.length);
    let worldId = worldIds[randomIndex];

    // let response = http.get(`http://localhost:55448/craft/${worldId}`);
    let response = http.get(`http://host.docker.internal:55448/item/1639`);

    check(response, {
        'status is 200': (r) => r.status === 200,
    });

    sleep(delay);
}
