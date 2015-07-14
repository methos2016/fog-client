#!/bin/bash
die () {
    echo >&2 "$@"
    exit 1
}
[ "$#" -eq 1 ] || die "1 argument required, $# provided"

critical_commands=(mono mono-service unzip wget);
optional_commands=(xprintidle);

echo "Checking required dependencies..."

for i in ${critical_commands[@]}; do
        if [ hash $i 2>/dev/null ]; then
			printf "%-20s %-30s" $i "Failed";
			exit 1;
		else
			printf "%-20s %-30s" $i "Success";
		fi
		echo "";
done

echo ""	
echo "Checking optional dependencies..."
for i in ${optional_commands[@]}; do
        if [ hash $i 2>/dev/null ]; then
			printf "%-20s %-30s" $i "Failed";
		else
			printf "%-20s %-30s" $i "Success";
		fi
		echo "";
done

