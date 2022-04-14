module.exports = {
	parser: 'babel-eslint',
	extends: [
		'eslint:recommended',
		'plugin:import/errors',
		'plugin:import/typescript',
		'plugin:react/recommended',
		'plugin:react/jsx-runtime',
	],
	overrides: [{
		files: ['**/*.ts', '**/*.tsx'],
		// TypeScript rules
		rules: {},
	}],
	plugins: [
		'prettier',
	],
	rules: {
		'semi': ['warn', 'always'],
		'quotes': ['warn', 'single'],
		'no-var': 'error',
		'curly': 'warn',
		'comma-dangle': ['warn', {
			'arrays': 'always-multiline',
			'objects': 'always-multiline',
			'imports': 'always-multiline',
			'exports': 'always-multiline',
			'functions': 'never',
		}],
		'arrow-parens': ['warn', 'always'],
		'prefer-destructuring': ['warn',
			{'array': true, 'object': true},
			{'enforceForRenamedProperties': false},
		],
		'prefer-const': ['warn', {'destructuring': 'all'}],
		'prefer-template': 'warn',
		'prefer-spread': 'warn',
		'prefer-rest-params': 'warn',
		'prefer-arrow-callback': 'warn',
		'no-multi-spaces': 'warn',
		'no-useless-return': 'warn',
		'no-trailing-spaces': 'warn',
		'no-console': 'warn',
		'no-constant-condition': ['error', {'checkLoops': false}],
		'no-unneeded-ternary': 'warn',
		'indent': [2, 4, {'SwitchCase': 1}],
		'import/no-absolute-path': 'error',
		'import/no-useless-path-segments': ['warn', {noUselessIndex: true}],
		'import/no-deprecated': 'warn',
		'import/no-unresolved': 'off',
		'import/first': 'error',
		'import/extensions': ['warn', 'never'],
		'import/order': ['warn', {'groups': ['builtin', 'external', 'internal', 'parent', 'sibling', 'index']}],
		'import/newline-after-import': 'warn',
		'max-len': ['warn', {
			'code': 120,
			'ignoreComments': true,
			'ignoreUrls': true,
			'ignoreStrings': true,
			'ignoreTemplateLiterals': true,
			'ignoreRegExpLiterals': true,
		}],
		'eol-last': ['warn', 'always'],
		'no-multiple-empty-lines': ['warn', {'max': 1, 'maxEOF': 1, 'maxBOF': 0}],
		'lines-between-class-members': ['warn', 'always'],
		'lines-around-directive': ['warn', {'before': 'never', 'after': 'always'}],
		'padded-blocks': ['warn', 'never'],
		// https://eslint.org/docs/rules/padding-line-between-statements
		'padding-line-between-statements': [
			'warn',
			{blankLine: 'always', prev: ['block', 'block-like'], next: '*'},
		],
		'keyword-spacing': ['warn'],
		'react/jsx-tag-spacing': ['warn'],
		'react/prop-types': [0],
		'react/display-name': [0],
	},
	'settings': {
		'import/resolver': {
			'node': {
				'paths': ['src'],
			},
		},
	},
};
