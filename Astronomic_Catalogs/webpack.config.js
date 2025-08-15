const path = require("path");
const fs = require("fs");
const TerserPlugin = require("terser-webpack-plugin");
const removeDebugLogs = 2; 

const entry = {
    alertOfSwal: "./Scripts/ts/alertOfSwal.ts",
    behavior: "./Scripts/ts/behavior.ts",
    calendarHandler: "./Scripts/ts/calendarHandler.ts",
    deviceUtils: "./Scripts/ts/deviceUtils.ts",
    formHandler: "./Scripts/ts/formHandler.ts",
    main: "./Scripts/ts/main.ts",
    metrics: "./Scripts/ts/metrics.ts",
    planetarySystemVisualization: "./Scripts/ts/planetarySystemVisualization.ts",
    progresImportBar: "./Scripts/ts/progresImportBar.ts",
    scriptTS: "./Scripts/ts/scriptTS.ts",
    switchTableType: "./Scripts/ts/switchTableType.ts",
    telescopeViewCalculator: "./Scripts/ts/telescopeViewCalculator.ts",
    theme: "./Scripts/ts/theme.ts",
};

// To avoid building in CI/CD.
const filesToCheck = {
    astroTableFilters: "./wwwroot/js/AstroTableFilters.js",
    development: "./Scripts/ts/development.ts",
};

for (const [key, relativePath] of Object.entries(filesToCheck)) {
    const fullPath = path.resolve(__dirname, relativePath);
    if (fs.existsSync(fullPath)) {
        entry[key] = relativePath;
    }
}

module.exports = {
    mode: "production",
    entry,
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
                    compress: (() => {
                        switch (removeDebugLogs) {
                            case 0:
                                return { drop_console: false };
                            case 1:
                                return { pure_funcs: ["console.log", "console.info", "console.debug"] };
                            default:
                                return { drop_console: true };
                        }
                    })(),
                    format: {
                        comments: false,
                    },
                },
                extractComments: false,
            }),
        ],
    },
    performance: {
        maxAssetSize: 512000,
        maxEntrypointSize: 512000
    },
};
