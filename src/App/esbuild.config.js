import esbuild from 'esbuild';
import Yargs from 'yargs';
import { hideBin } from 'yargs/helpers';

const Arguments = Yargs(hideBin(process.argv)).options({
  configuration: {
    default: 'Debug',
    type: 'string'
  },

  inputs: {
    demandOption: true,
    requiresArg: true,
    type: 'string'
  },

  output: {
    demandOption: true,
    requiresArg: true,
    type: 'string'
  },

  watch: {
    default: false,
    type: 'boolean',
  }
}).parse();

/** @returns {esbuild.BuildOptions} */
const useConfig = () => ({
  bundle: true,
  define: {
    'process.env.BUILD_CONFIGURATION': Arguments.configuration,
    'process.env.NODE_ENV': Arguments.configuration === 'Debug' ? '"development"' : '"production"'
  },
  entryPoints: Arguments.inputs.split(';'),
  format: 'esm',
  loader: {
    '.ts': 'ts',
  },
  metafile: true,
  minify: true,
  outdir: Arguments.output,
  plugins: [],
  sourcemap: Arguments.configuration === 'Debug',
  target: 'es6',
  tsconfig: 'tsconfig.json',
});

if (!Arguments.watch) {
  const result = await esbuild.build(useConfig());
  if (result.errors?.length) {
    console.error(result.errors);
    process.exit(-1);
  } else {
    for (const output in result.metafile.outputs) {
      console.log(output);
    }
  }
} else {
  await esbuild.context(useConfig()).then(context => context.watch());
}