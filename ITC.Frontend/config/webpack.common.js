const { CleanWebpackPlugin } = require("clean-webpack-plugin");

const HtmlWebpackPlugin = require("html-webpack-plugin");
// const ForkTsCheckerWebpackPlugin = require("fork-ts-checker-webpack-plugin");
const paths = require("./paths");
const { resolve } = require("path");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

const devServer = {
  historyApiFallback: true, // Apply HTML5 History API if paths are used
  open: true,
  compress: true,
  allowedHosts: 'all',
  proxy: [
    // SignalR
    {
      context: ['/engineHub'],
      changeOrigin: true,
      target: 'http://89.108.73.166:5016/engineHub',
      ws: true,
    },
  ],
  headers: {
    'Access-Control-Allow-Origin': '*',
    'Access-Control-Allow-Methods': '*',
    'Access-Control-Allow-Headers': '*',
  },
  client: {
    // Shows a full-screen overlay in the browser when there are compiler errors or warnings
    overlay: {
      errors: true,
      warnings: true,
    },
  },
  port: 3001,
}

module.exports = {
  entry: [paths.src + "/index.tsx"],
  devServer,
  mode: "development",
  output: {
    path: paths.build,
    filename: "[name].bundle.js",
    publicPath: "/",
  },

  plugins: [
    new MiniCssExtractPlugin(),
    new CleanWebpackPlugin(),
    // Build with checking
    // new ForkTsCheckerWebpackPlugin(),

    new HtmlWebpackPlugin({
      title: "webpack Boilerplate",
      favicon: paths.src + "/images/favicon.svg",
      template: paths.src + "../../public/index.html",
      filename: "index.html",
    }),
  ],
  module: {
    rules: [
      {
        test: /\.tsx?$/,
        use: "ts-loader",
        exclude: /node_modules/,
      },
      {
        test: /\.less$/i,
        use: [
          // compiles Less to CSS
          MiniCssExtractPlugin.loader,
          "css-loader",
          "less-loader",
        ],
      },
      {
        test: /\.css$/i,
        exclude: [
          resolve(__dirname, "../node_modules"),
          resolve(__dirname, "../src/index.css"),
        ],
        use: [
          MiniCssExtractPlugin.loader,
          {
            loader: require.resolve("css-loader"),
            options: {
              importLoaders: 1,
              modules: {
                localIdentName: "[name]__[local]___[hash:base64:5]",
                mode: "local",
              },
            },
          },
        ],
      },
      {
        test: /\.css$/i,
        include: [
          resolve(__dirname, "../node_modules"),
          resolve(__dirname, "../src/index.css"),
        ],
        use: [
          MiniCssExtractPlugin.loader,
          {
            loader: require.resolve("css-loader"),
            options: {
              importLoaders: 1,
              modules: {
                localIdentName: "[local]",
              },
            },
          },
        ],
      },
      { test: /\.(?:ico|gif|png|jpg|jpeg|xlsx)$/i, type: "asset/resource" },
      { test: /\.(woff(2)?|eot|ttf|otf|svg|)$/, type: "asset/inline" },
    ],
  },
  resolve: {
    modules: [paths.src, "node_modules"],
    extensions: [".js", ".jsx", ".ts", ".tsx"],
    alias: {
      "@": paths.src,
    },
  },
};
