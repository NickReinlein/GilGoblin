module.exports = {
    testEnvironment: 'jsdom',
    preset: 'ts-jest',
    // transform: {
    //     '^.+\\.jsx?$': 'babel-jest'
    // },
    testMatch: [
        "**/__tests__/**/*.ts?(x)",
        "**/?(*.)+(spec|test).ts?(x)"
    ]
};
