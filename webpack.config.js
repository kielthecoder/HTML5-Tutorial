const path = require('path');

const CopyPlugin = require('copy-webpack-plugin');

module.exports = function (env) {
    if (env.target === undefined) {
        env.target = '';
    }

    if (env.source === undefined) {
        env.source = 'src';
    }

    return {
        mode: 'development',
        entry: './' + env.source + '/app.js',
        output: {
            filename: 'bundle.js',
            path: path.resolve(__dirname, env.target, 'dist')
        },
        plugins: [
            new CopyPlugin({
                patterns: [
                    {
                        from: env.source + '/assets',
                        to: 'assets/'
                    },
                    {
                        from: env.source + '/*.(html|css)',
                        to: '[name][ext]'
                    }
                ]
            })
        ]
    };
};