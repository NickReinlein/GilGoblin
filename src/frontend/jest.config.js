module.exports = {
    testEnvironment: 'jsdom',
    preset: 'ts-jest',
    transform: {
        '^.+\\.jsx?$': 'babel-jest',
        '^.+\\.tsx?$': 'ts-jest'
    },
    testMatch: [
        "**/*.spec.ts?(x)",
    ],
    collectCoverage: true,
    coveragePathIgnorePatterns: [
        "/node_modules/",
        "index.tsx",
        "reportWebVitals.ts"
    ],
};