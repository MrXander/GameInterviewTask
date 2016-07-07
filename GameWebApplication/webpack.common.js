import path from 'path';
import autoprefixer from 'autoprefixer';

export default {
    entry: {
        'app': './scripts/react/app.js'
    },
    output: {
        path: path.join(__dirname, 'dist'),
        filename: '[name].js'
    },
    resolve: {
        extensions: [ '', '.js' ]
    },
    module: {
        loaders: [
            {
                test: /\.js$/,
                exclude: /node_modules/,
                loader: 'babel',
                query: { cacheDirectory: true }
            },
            {
                test: /\.css$/,
                loaders: [ 'style', 'css', 'postcss' ]
            }
        ]
    },
    postcss: () => [
        autoprefixer()
    ]
};
