import * as Toast from '@radix-ui/react-toast';
export default function ProcessPageLayout({
    children,
}: {
    children: React.ReactNode;
}) {
    return (<><Toast.Provider>
        <div className="min-h-screen bg-gray-100 p-6">
            {children}
        </div>
        <Toast.Viewport className="fixed bottom-0 right-0 p-4" />
    </Toast.Provider></>
    );
}
