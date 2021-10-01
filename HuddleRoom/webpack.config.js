const path = require('path');

const CopyPlugin = require('copy-webpack-plugin');

module.exports = {
    mode: 'development',
    entry: './html/app.js',
    output: {
        filename: 'bundle.js',
        path: path.resolve(__dirname, 'dist')
    },
    plugins: [
        new CopyPlugin({
            patterns: [
                {
                    from: 'html/assets',
                    to: 'assets/'
                },
                {
                    from: 'html/*.(html|css)',
                    to: '[name][ext]'
                }
            ]
        })
    ]
}