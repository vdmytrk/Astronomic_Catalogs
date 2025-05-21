const path = require("path");
const TerserPlugin = require("terser-webpack-plugin");

module.exports = {
    mode: "production",
    entry: {
        alertOfSwal: "./Scripts/ts/alertOfSwal.ts",
        behavior: "./Scripts/ts/behavior.ts",
        development: "./Scripts/ts/development.ts",
        formHandler: "./Scripts/ts/formHandler.ts",
        main: "./Scripts/ts/main.ts",
        metrics: "./Scripts/ts/metrics.ts",
        progresImportBar: "./Scripts/ts/progresImportBar.ts",
        scriptTS: "./Scripts/ts/scriptTS.ts",
        theme: "./Scripts/ts/theme.ts",
        astroTableFilters: "./wwwroot/js/AstroTableFilters.js",
        telescopeViewCalculator: "./Scripts/ts/telescopeViewCalculator.ts",
        calendarHandler: "./Scripts/ts/calendarHandler.ts",
        switchTableType: "./Scripts/ts/switchTableType.ts",
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
        alias: {
            '@modules': path.resolve(__dirname, "wwwroot/js/")
        },
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
                        drop_console: false,
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

