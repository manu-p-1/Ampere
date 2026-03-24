import sys

path = '/Users/mp/Github/Ampere/Ampere/Str/StringUtils.cs'
with open(path, 'r') as f:
    lines = f.readlines()

new_lines = []
in_namespace = False
namespace_depth = 0

for i, line in enumerate(lines):
    stripped = line.rstrip('\n')

    if stripped == 'namespace Ampere.Str':
        new_lines.append('namespace Ampere.Str;\n')
        in_namespace = True
        continue

    if in_namespace and stripped == '{' and namespace_depth == 0:
        new_lines.append('\n')
        namespace_depth = 1
        continue

    if in_namespace and stripped == '} //Note':
        continue

    if in_namespace and namespace_depth > 0:
        if line.startswith('    '):
            new_lines.append(line[4:])
        elif stripped == '':
            new_lines.append('\n')
        else:
            new_lines.append(line)
    else:
        new_lines.append(line)

with open(path, 'w') as f:
    f.writelines(new_lines)

print('Done - converted to file-scoped namespace')
