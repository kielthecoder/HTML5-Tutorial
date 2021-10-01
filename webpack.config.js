const path = require('path');

const CopyPlugin = require('copy-webpack-plugin');

module.exports = function (env) {
    if (env.name === undefined) {
        env.name = '';
    }

    if (env.prefix === undefined) {
        env.prefix = 'src/';
    }

    return {
        mode: 'development',
        entry: './' + env.prefix + 'app.js',
        output: {
            filename: 'bundle.js',
            path: path.resolve(__dirname, env.name, 'dist')
        },
        plugins: [
            new CopyPlugin({
                patterns: [
                    {
                        from: env.prefix + 'assets',
                        to: 'assets/'
                    },
                    {
                        from: env.prefix + '*.(html|css)',
                        to: '[name][ext]'
                    }
                ]
            })
        ]
    };
};