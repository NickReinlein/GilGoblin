import http from "k6/http";
import {check, sleep} from "k6";

export let options = {
  stages: [
    { duration: "3m", target: 50 }, // Ramp-up to 50 VUs over 3 minutes
    { duration: "3m", target: 100 }, // Ramp-up to 100 VUs over 3 minutes
    { duration: "10m", target: 100 }, // Hold 100 VUs for 10 minutes
    { duration: "2m", target: 50 }, // Ramp-down to 50 VUs over 2 minutes
    { duration: "2m", target: 0 }, // Ramp-down to 0 VUs over 5 minutes
  ],
  ext: {
    thresholds: {
      http_req_duration: ["p(99)<500"], // 99% of requests must complete below 500ms
      http_req_failed: ["rate<0.02"], // http errors should be less than 2%
    },
    metricsExporters: {
      prometheus: {
        address: "host.docker.internal:9090",
      },
    },
  },
};

const worldIds = [21, 22, 23, 34];

export default function () {
    const delay = 500;

    let randomIndex = Math.floor(Math.random() * worldIds.length);
    let worldId = worldIds[randomIndex];

    let response = http.get(`http://host.docker.internal:55448/craft/${worldId}`);

    check(response, {
        "status is 200": (r) => r.status === 200,
    });
1;
    sleep(delay);
}
