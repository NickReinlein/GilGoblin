module.exports = {
    testEnvironment: 'jsdom',
    preset: 'ts-jest',
    transform: {
        '^.+\\.jsx?$': 'babel-jest'
    },
    testMatch: [
        "**/*.spec.ts?(x)",
    ]
};