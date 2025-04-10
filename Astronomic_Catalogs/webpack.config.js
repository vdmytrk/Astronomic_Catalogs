const path = require("path");
const TerserPlugin = require("terser-webpack-plugin");

module.exports = {
    mode: "production",
    entry: {
        main: "./Scripts/ts/main.ts",
        theme: "./wwwroot/js/theme.js",
    },
    output: {
        filename: "[name].min.js",
        path: path.resolve(__dirname, "wwwroot/js"),
    },
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                use: "ts-loader",
                exclude: /node_modules/,
            },
        ],
    },
    resolve: {
        extensions: [".ts", ".tsx", ".js"],
        modules: [
            path.resolve(__dirname, "node_modules"),
            "node_modules"
        ]
    },
    optimization: {
        minimize: true,
        minimizer: [
            new TerserPlugin({
                terserOptions: {
                    compress: {
                        drop_console: true,
                    },
                    format: {
                        comments: false,
                    },
                },
                extractComments: false,
            }),
        ],
    },
};

